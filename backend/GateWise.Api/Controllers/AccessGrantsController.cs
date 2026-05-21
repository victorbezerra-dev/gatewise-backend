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
    private readonly ILabRepository _labRepository;

    public AccessGrantsController(
        IAccessGrantRepository accessGrantRepository,
        IUserRepository userRepository,
        ILabRepository labRepository)
    {
        _accessGrantRepository = accessGrantRepository;
        _userRepository = userRepository;
        _labRepository = labRepository;
    }

    [HttpGet]
    [Authorize(Roles = "admin,professor")]
    public async Task<ActionResult<IEnumerable<AccessGrantResponseDto>>> GetAll([FromQuery] string? search)
    {
        var accessGrants = await _accessGrantRepository.GetAllAsync(search);
        var result = accessGrants.Select(MapToDto);
        return Ok(result);
    }


    [HttpGet("{id}")]
    [Authorize(Roles = "admin,professor")]
    public async Task<ActionResult<AccessGrantResponseDto>> GetById(int id)
    {
        var accessGrant = await _accessGrantRepository.GetByIdAsync(id);
        if (accessGrant is null)
            return NotFound();

        return Ok(MapToDto(accessGrant));
    }

    [HttpPost("request-access")]
    [Authorize]
    public async Task<ActionResult> Create([FromBody] CreateAccessGrantDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var user = await _userRepository.GetByIdAsync(userId);
        var lab = await _labRepository.GetByIdAsync(dto.LabId);

        if (user is null || lab is null)
            return BadRequest("Invalid user or LabId.");

        var allAccessGrants = await _accessGrantRepository.GetAllAsync();
        var alreadyExists = allAccessGrants.Any(g =>
            g.AuthorizedUserId == userId &&
            g.LabId == dto.LabId);

        if (alreadyExists)
            return Conflict("Access request already exists.");

        var newAccessGrant = new AccessGrant
        {
            AuthorizedUserId = userId,
            LabId = dto.LabId,
            Reason = dto.Reason,
            Status = AccessGrantStatus.Pending,
            GrantedAt = DateTime.UtcNow
        };

        await _accessGrantRepository.AddAsync(newAccessGrant);
        return CreatedAtAction(nameof(GetById), new { id = newAccessGrant.Id }, MapToDto(newAccessGrant));
    }

    [HttpPut("{id}/review")]
    [Authorize(Roles = "admin,professor")]
    public async Task<ActionResult> ReviewAccessGrant(int id, [FromBody] ReviewAccessGrantDto dto)
    {
        var accessGrant = await _accessGrantRepository.GetByIdAsync(id);
        if (accessGrant is null)
            return NotFound();

        if (dto.Status == AccessGrantStatus.Pending)
            return BadRequest("Cannot revert to 'Pending' status.");

        var grantedByUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        accessGrant.GrantedByUserId = grantedByUserIdStr;
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
    [Authorize(Roles = "admin,professor")]
    public async Task<ActionResult> Delete(int id)
    {
        var accessGrant = await _accessGrantRepository.GetByIdAsync(id);
        if (accessGrant is null)
            return NotFound();

        await _accessGrantRepository.DeleteAsync(accessGrant);
        return NoContent();
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AccessGrantResponseDto>>> GetByUserId(string userId)
    {
        var matricula = User.FindFirst("preferred_username")?.Value;
        if (string.IsNullOrWhiteSpace(matricula))
            return Forbid();

        var currentUser = await _userRepository.GetByEmailOrRegistrationAsync("", matricula);
        if (currentUser is null)
            return Forbid();

        var callerId = currentUser.Id;
        var isAdmin = User.IsInRole("admin");
        var isProfessor = User.IsInRole("professor");

        var isSelf = callerId == userId;

        if (!isAdmin && !isSelf && !isProfessor)
            return Forbid();

        var accessGrants = await _accessGrantRepository.GetByUserIdAsync(userId);

        return Ok(accessGrants.Select(MapToDto));
    }


    private static AccessGrantResponseDto MapToDto(AccessGrant accessGrant) => new()
    {
        Id = accessGrant.Id,
        AuthorizedUserId = accessGrant.AuthorizedUserId,
        AuthorizedUserName = accessGrant.AuthorizedUser.Name,
        GrantedByUserId = accessGrant.GrantedByUserId ?? "",
        GrantedByUserName = accessGrant.GrantedByUser?.Name ?? string.Empty,
        LabId = accessGrant.LabId,
        LabName = accessGrant.Lab.Name,
        GrantedAt = accessGrant.GrantedAt,
        RevokedAt = accessGrant.RevokedAt,
        Reason = accessGrant.Reason,
        Status = accessGrant.Status
    };
}
