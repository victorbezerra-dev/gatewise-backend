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
    private readonly ISpaceManagerRepository _spaceManagerRepository;

    public AccessGrantsController(
        IAccessGrantRepository accessGrantRepository,
        IUserRepository userRepository,
        ISpaceRepository spaceRepository,
        IOrganizationMemberRepository memberRepository,
        ISpaceManagerRepository spaceManagerRepository)
    {
        _accessGrantRepository = accessGrantRepository;
        _userRepository = userRepository;
        _spaceRepository = spaceRepository;
        _memberRepository = memberRepository;
        _spaceManagerRepository = spaceManagerRepository;
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

        var ownerOrgIds = memberships
            .Where(m => m.Role == OrganizationMemberRole.Owner)
            .Select(m => m.OrganizationId)
            .ToHashSet();

        var assignedSpaceIds = (await _spaceManagerRepository.GetByUserIdAsync(userId))
            .Select(sm => sm.SpaceId)
            .ToHashSet();

        var grants = await _accessGrantRepository.GetAllAsync(search);
        var filtered = grants.Where(g =>
            ownerOrgIds.Contains(g.Space.OrganizationId) ||
            assignedSpaceIds.Contains(g.SpaceId));
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
            if (member is null) return Forbid();

            if (member.Role == OrganizationMemberRole.Manager && !await _spaceManagerRepository.IsManagerOfSpaceAsync(accessGrant.SpaceId, userId))
                return Forbid();

            if (member.Role != OrganizationMemberRole.Owner && member.Role != OrganizationMemberRole.Manager)
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
        var space = await _spaceRepository.GetByIdAsync(dto.SpaceId);

        if (user is null || space is null)
            return BadRequest("Invalid user or SpaceId.");

        var member = await _memberRepository.GetAsync(space.OrganizationId, userId);
        if (member is null)
            return Forbid();

        var allAccessGrants = await _accessGrantRepository.GetAllAsync();
        var alreadyExists = allAccessGrants.Any(g =>
            g.AuthorizedUserId == userId && g.SpaceId == dto.SpaceId);

        if (alreadyExists)
            return Conflict("Access request already exists.");

        var newAccessGrant = new AccessGrant
        {
            AuthorizedUserId = userId,
            SpaceId = dto.SpaceId,
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
            if (member is null) return Forbid();

            if (member.Role == OrganizationMemberRole.Manager && !await _spaceManagerRepository.IsManagerOfSpaceAsync(accessGrant.SpaceId, userId))
                return Forbid();

            if (member.Role != OrganizationMemberRole.Owner && member.Role != OrganizationMemberRole.Manager)
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
            accessGrant.RevokedAt = DateTime.UtcNow;
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
                if (!await _spaceManagerRepository.IsManagerOfSpaceAsync(accessGrant.SpaceId, callerId))
                    return Forbid();

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
        await RemoveIfOrphanedAsync(accessGrant.Space.OrganizationId, accessGrant.AuthorizedUserId);
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

    private async Task RemoveIfOrphanedAsync(int orgId, string userId)
    {
        var member = await _memberRepository.GetAsync(orgId, userId);
        if (member is null || member.Role == OrganizationMemberRole.Owner)
            return;

        var hasGrants = (await _accessGrantRepository.GetByUserIdAsync(userId))
            .Any(g => g.Space.OrganizationId == orgId);
        var hasManaged = (await _spaceManagerRepository.GetByUserIdAsync(userId))
            .Any(sm => sm.Space.OrganizationId == orgId);

        if (!hasGrants && !hasManaged)
            await _memberRepository.DeleteAsync(member);
    }

    private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();

}
