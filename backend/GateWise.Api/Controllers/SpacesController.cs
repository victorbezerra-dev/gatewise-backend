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
    private readonly IOrganizationMemberRepository _memberRepositorysitory;

    public SpacesController(
        ISpaceRepository spaceRepository,
        ISpaceAccessService spaceAccessService,
        IOrganizationMemberRepository memberRepository)
    {
        _spaceRepository = spaceRepository;
        _spaceAccessService = spaceAccessService;
        _memberRepositorysitory = memberRepository;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpaceResponseDto>>> GetAll()
    {
        if (User.IsInRole("admin"))
        {
            var allSpaces = await _spaceRepository.GetAllAsync();
            return Ok(allSpaces.Select(SpaceResponseDto.From));
        }

        var userId = GetUserId();
        var memberships = await _memberRepositorysitory.GetByUserIdAsync(userId);
        var orgIds = memberships.Select(m => m.OrganizationId).ToHashSet();

        if (orgIds.Count == 0)
            return Ok(Array.Empty<SpaceResponseDto>());

        var spaces = new List<Space>();
        foreach (var orgId in orgIds)
            spaces.AddRange(await _spaceRepository.GetByOrganizationIdAsync(orgId));

        return Ok(spaces.Select(SpaceResponseDto.From));
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
            var member = await _memberRepositorysitory.GetAsync(space.OrganizationId, userId);
            if (member is null) return Forbid();
        }

        return Ok(SpaceResponseDto.From(space));
    }

    [Authorize(Roles = "admin,manager")]
    [HttpPost]
    public async Task<ActionResult<SpaceResponseDto>> Create([FromBody] SpaceUpsertDto dto)
    {
        int organizationId;

        if (User.IsInRole("admin"))
        {
            if (dto.OrganizationId is null)
                return BadRequest("OrganizationId is required for admin.");
            organizationId = dto.OrganizationId.Value;
        }
        else
        {
            var userId = GetUserId();
            var memberships = await _memberRepositorysitory.GetByUserIdAsync(userId);
            var managerMembership = memberships.FirstOrDefault(m =>
                m.Role == OrganizationMemberRole.Owner || m.Role == OrganizationMemberRole.Manager);

            if (managerMembership is null)
                return Forbid();

            organizationId = dto.OrganizationId ?? managerMembership.OrganizationId;

            var membership = await _memberRepositorysitory.GetAsync(organizationId, userId);
            if (membership is null || (membership.Role != OrganizationMemberRole.Owner && membership.Role != OrganizationMemberRole.Manager))
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
        return CreatedAtAction(nameof(Get), new { id = space.Id }, SpaceResponseDto.From(space));
    }

    [HttpPost("{id}/open")]
    public async Task<IActionResult> OpenSpace(int id, [FromBody] AccessLogCreateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token.");

        var result = await _spaceAccessService.RequestSpaceAccessAsync(userId, id, dto);
        return Ok(new { commandId = result });
    }

    [HttpPost("access-confirmation")]
    public async Task<IActionResult> AccessConfirmation([FromBody] AccessLogConfirmDto dto)
    {
        var confirmed = await _spaceAccessService.ConfirmAccessAsync(dto);
        return confirmed ? Ok() : StatusCode(403, "Access not confirmed by app");
    }

    [Authorize(Roles = "admin,manager")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SpaceUpsertDto dto)
    {
        var space = await _spaceRepository.GetByIdAsync(id);
        if (space is null) return NotFound();

        if (!User.IsInRole("admin"))
        {
            var userId = GetUserId();
            var member = await _memberRepositorysitory.GetAsync(space.OrganizationId, userId);
            if (member is null || (member.Role != OrganizationMemberRole.Owner && member.Role != OrganizationMemberRole.Manager))
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

    [Authorize(Roles = "admin,manager")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var space = await _spaceRepository.GetByIdAsync(id);
        if (space is null) return NotFound();

        if (!User.IsInRole("admin"))
        {
            var userId = GetUserId();
            var member = await _memberRepositorysitory.GetAsync(space.OrganizationId, userId);
            if (member is null || (member.Role != OrganizationMemberRole.Owner && member.Role != OrganizationMemberRole.Manager))
                return Forbid();
        }

        await _spaceRepository.DeleteAsync(space);
        return NoContent();
    }

    private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();

}
