using GateWise.Application.DTOs;
using GateWise.Core.Entities;
using GateWise.Core.Enums;
using GateWise.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GateWise.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccessGrantsController : ControllerBase
{
    private readonly IAccessGrantRepository _accessGrantRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISpaceRepository _spaceRepository;
    private readonly IOrganizationMemberRepository _memberRepository;

    public AccessGrantsController(
        IAccessGrantRepository accessGrantRepository,
        IUserRepository userRepository,
        ISpaceRepository spaceRepository,
        IOrganizationMemberRepository memberRepository)
    {
        _accessGrantRepository = accessGrantRepository;
        _userRepository = userRepository;
        _spaceRepository = spaceRepository;
        _memberRepository = memberRepository;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AccessGrantResponseDto>>> GetAll([FromQuery] string? search)
    {
        if (User.IsInRole("admin"))
        {
            var all = await _accessGrantRepository.GetAllAsync(search);
            return Ok(all.Select(AccessGrantResponseDto.From));
        }

        var userId = GetUserId();
        var memberships = await _memberRepository.GetByUserIdAsync(userId);
        var orgIds = memberships
            .Where(m => m.Role == OrganizationMemberRole.Owner || m.Role == OrganizationMemberRole.Manager)
            .Select(m => m.OrganizationId)
            .ToHashSet();

        var grants = await _accessGrantRepository.GetAllAsync(search);
        var filtered = grants.Where(g => orgIds.Contains(g.Space.OrganizationId));
        return Ok(filtered.Select(AccessGrantResponseDto.From));
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<AccessGrantResponseDto>> GetById(int id)
    {
        var accessGrant = await _accessGrantRepository.GetByIdAsync(id);
        if (accessGrant is null) return NotFound();

        if (!User.IsInRole("admin"))
        {
            var userId = GetUserId();
            var member = await _memberRepository.GetAsync(accessGrant.Space.OrganizationId, userId);
            if (member is null || (member.Role != OrganizationMemberRole.Owner && member.Role != OrganizationMemberRole.Manager))
                return Forbid();
        }

        return Ok(AccessGrantResponseDto.From(accessGrant));
    }

    [HttpPost("request-access")]
    [Authorize]
    public async Task<ActionResult> Create([FromBody] CreateAccessGrantDto dto)
    {
        var userId = GetUserId();

        var user = await _userRepository.GetByIdAsync(userId);
        var space = await _spaceRepository.GetByIdAsync(dto.LabId);

        if (user is null || space is null)
            return BadRequest("Invalid user or SpaceId.");

        var member = await _memberRepository.GetAsync(space.OrganizationId, userId);
        if (member is null)
            return Forbid();

        var allAccessGrants = await _accessGrantRepository.GetAllAsync();
        var alreadyExists = allAccessGrants.Any(g =>
            g.AuthorizedUserId == userId && g.SpaceId == dto.LabId);

        if (alreadyExists)
            return Conflict("Access request already exists.");

        var newAccessGrant = new AccessGrant
        {
            AuthorizedUserId = userId,
            SpaceId = dto.LabId,
            Reason = dto.Reason,
            Status = AccessGrantStatus.Pending,
            GrantedAt = DateTime.UtcNow
        };

        await _accessGrantRepository.AddAsync(newAccessGrant);
        return CreatedAtAction(nameof(GetById), new { id = newAccessGrant.Id }, AccessGrantResponseDto.From(newAccessGrant));
    }

    [HttpPut("{id}/review")]
    [Authorize]
    public async Task<ActionResult> ReviewAccessGrant(int id, [FromBody] ReviewAccessGrantDto dto)
    {
        var accessGrant = await _accessGrantRepository.GetByIdAsync(id);
        if (accessGrant is null) return NotFound();

        if (!User.IsInRole("admin"))
        {
            var userId = GetUserId();
            var member = await _memberRepository.GetAsync(accessGrant.Space.OrganizationId, userId);
            if (member is null || (member.Role != OrganizationMemberRole.Owner && member.Role != OrganizationMemberRole.Manager))
                return Forbid();
        }

        if (dto.Status == AccessGrantStatus.Pending)
            return BadRequest("Cannot revert to 'Pending' status.");

        accessGrant.GrantedByUserId = GetUserId();
        accessGrant.Status = dto.Status;
        accessGrant.Reason = dto.Reason ?? accessGrant.Reason;

        if (dto.Status == AccessGrantStatus.Granted)
        {
            accessGrant.GrantedAt = DateTime.UtcNow;
            accessGrant.RevokedAt = null;
        }
        else if (dto.Status == AccessGrantStatus.Rejected)
        {
            accessGrant.RevokedAt = dto.RevokedAt ?? DateTime.UtcNow;
        }

        await _accessGrantRepository.UpdateAsync(accessGrant);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var accessGrant = await _accessGrantRepository.GetByIdAsync(id);
        if (accessGrant is null) return NotFound();

        var callerId = GetUserId();

        if (!User.IsInRole("admin") && accessGrant.AuthorizedUserId != callerId)
        {
            var callerMember = await _memberRepository.GetAsync(accessGrant.Space.OrganizationId, callerId);
            if (callerMember is null) return Forbid();

            if (callerMember.Role == OrganizationMemberRole.Manager)
            {
                var targetMember = await _memberRepository.GetAsync(accessGrant.Space.OrganizationId, accessGrant.AuthorizedUserId);
                if (targetMember?.Role == OrganizationMemberRole.Owner)
                    return Forbid();
            }
            else if (callerMember.Role != OrganizationMemberRole.Owner)
            {
                return Forbid();
            }
        }

        await _accessGrantRepository.DeleteAsync(accessGrant);
        return NoContent();
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AccessGrantResponseDto>>> GetByUserId(string userId)
    {
        var callerId = GetUserId();
        var isSelf = callerId == userId;

        if (!User.IsInRole("admin") && !User.IsInRole("manager") && !isSelf)
            return Forbid();

        var accessGrants = await _accessGrantRepository.GetByUserIdAsync(userId);
        return Ok(accessGrants.Select(AccessGrantResponseDto.From));
    }

    private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();

}
