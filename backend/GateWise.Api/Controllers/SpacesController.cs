using System.Security.Claims;
using GateWise.Core.Dtos;
using GateWise.Core.DTOs;
using GateWise.Core.Entities;
using GateWise.Core.Enums;
using GateWise.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SpacesController : ControllerBase
{
    private readonly ISpaceRepository _spaceRepository;
    private readonly ISpaceAccessService _spaceAccessService;
    private readonly IOrganizationMemberRepository _memberRepository;
    private readonly ISpaceManagerRepository _spaceManagerRepository;

    public SpacesController(
        ISpaceRepository spaceRepository,
        ISpaceAccessService spaceAccessService,
        IOrganizationMemberRepository memberRepository,
        ISpaceManagerRepository spaceManagerRepository)
    {
        _spaceRepository = spaceRepository;
        _spaceAccessService = spaceAccessService;
        _memberRepository = memberRepository;
        _spaceManagerRepository = spaceManagerRepository;
    }

    [Authorize]
    [HttpGet("/api/organizations/{organizationId}/spaces")]
    public async Task<ActionResult<IEnumerable<SpaceResponseDto>>> GetByOrganization(int organizationId)
    {
        if (!User.IsInRole("admin"))
        {
            var userId = GetUserId();
            var membership = await _memberRepository.GetAsync(organizationId, userId);
            if (membership is null) return Forbid();
        }

        var spaces = await _spaceRepository.GetByOrganizationIdAsync(organizationId);
        return Ok(spaces.Select(SpaceResponseDto.From));
    }

    [Authorize]
    [HttpPost("/api/organizations/{organizationId}/spaces")]
    public async Task<ActionResult<SpaceResponseDto>> Create(int organizationId, [FromBody] SpaceUpsertDto dto)
    {
        if (!User.IsInRole("admin"))
        {
            var userId = GetUserId();
            var membership = await _memberRepository.GetAsync(organizationId, userId);
            if (membership is null || membership.Role != OrganizationMemberRole.Owner)
                return Forbid();
        }

        var space = new Space
        {
            OrganizationId = organizationId,
            Name = dto.Name,
            Code = dto.Code,
            ImageUrl = dto.ImageUrl,
            Description = dto.Description,
            Location = dto.Location,
            Floor = dto.Floor,
            Building = dto.Building,
            Capacity = dto.Capacity,
            IsActive = dto.IsActive,
            OpenTime = dto.OpenTime,
            CloseTime = dto.CloseTime,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _spaceRepository.AddAsync(space);
        return Ok(SpaceResponseDto.From(space));
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<SpaceResponseDto>> Get(int id)
    {
        var space = await _spaceRepository.GetByIdAsync(id);
        if (space is null) return NotFound();

        if (!User.IsInRole("admin"))
        {
            var userId = GetUserId();
            var member = await _memberRepository.GetAsync(space.OrganizationId, userId);
            if (member is null) return Forbid();
        }

        return Ok(SpaceResponseDto.From(space));
    }

    [HttpPost("{id}/open")]
    public async Task<IActionResult> OpenSpace(int id, [FromBody] AccessLogCreateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token.");

        var space = await _spaceRepository.GetByIdAsync(id);
        if (space is null) return NotFound();

        var member = await _memberRepository.GetAsync(space.OrganizationId, userId);
        if (member is null) return Forbid();

        var now = DateTime.UtcNow;
        if (member.StartsAt.HasValue && now < member.StartsAt.Value)
            return Forbid();
        if (member.ExpiresAt.HasValue && now > member.ExpiresAt.Value)
            return Forbid();

        var result = await _spaceAccessService.RequestSpaceAccessAsync(userId, id, dto);
        return Ok(new { commandId = result });
    }

    [HttpPost("access-confirmation")]
    public async Task<IActionResult> AccessConfirmation([FromBody] AccessLogConfirmDto dto)
    {
        var confirmed = await _spaceAccessService.ConfirmAccessAsync(dto);
        return confirmed ? Ok() : StatusCode(403, "Access not confirmed by app");
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SpaceUpsertDto dto)
    {
        var space = await _spaceRepository.GetByIdAsync(id);
        if (space is null) return NotFound();

        if (!User.IsInRole("admin"))
        {
            var userId = GetUserId();
            var member = await _memberRepository.GetAsync(space.OrganizationId, userId);
            if (member is null) return Forbid();

            if (member.Role == OrganizationMemberRole.Manager && !await _spaceManagerRepository.IsManagerOfSpaceAsync(space.Id, userId))
                return Forbid();

            if (member.Role != OrganizationMemberRole.Owner && member.Role != OrganizationMemberRole.Manager)
                return Forbid();
        }

        space.Name = dto.Name;
        space.Code = dto.Code;
        space.ImageUrl = dto.ImageUrl;
        space.Description = dto.Description;
        space.Location = dto.Location;
        space.Floor = dto.Floor;
        space.Building = dto.Building;
        space.Capacity = dto.Capacity;
        space.IsActive = dto.IsActive;
        space.OpenTime = dto.OpenTime;
        space.CloseTime = dto.CloseTime;
        space.UpdatedAt = DateTime.UtcNow;

        await _spaceRepository.UpdateAsync(space);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var space = await _spaceRepository.GetByIdAsync(id);
        if (space is null) return NotFound();

        if (!User.IsInRole("admin"))
        {
            var userId = GetUserId();
            var member = await _memberRepository.GetAsync(space.OrganizationId, userId);
            if (member is null) return Forbid();

            if (member.Role == OrganizationMemberRole.Manager && !await _spaceManagerRepository.IsManagerOfSpaceAsync(space.Id, userId))
                return Forbid();

            if (member.Role != OrganizationMemberRole.Owner && member.Role != OrganizationMemberRole.Manager)
                return Forbid();
        }

        await _spaceRepository.DeleteAsync(space);
        return NoContent();
    }

    private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();

}
