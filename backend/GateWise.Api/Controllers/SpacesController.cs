using System.Security.Claims;
using GateWise.Core.Dtos;
using GateWise.Core.DTOs;
using GateWise.Core.Entities;
using GateWise.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SpacesController : ControllerBase
{
    private readonly ISpaceRepository _spaceRepository;
    private readonly ISpaceAccessService _spaceAccessService;

    public SpacesController(ISpaceRepository spaceRepository, ISpaceAccessService spaceAccessService, IAccessLogRepository accessLogRepository)
    {
        _spaceRepository = spaceRepository;
        _spaceAccessService = spaceAccessService;
    }

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var spaces = await _spaceRepository.GetAllAsync();
        return Ok(spaces);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var space = await _spaceRepository.GetByIdAsync(id);
        return space is null ? NotFound() : Ok(space);
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SpaceUpsertDto dto)
    {
        var space = new Space
        {
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
        return CreatedAtAction(nameof(Get), new { id = space.Id }, space);
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

    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SpaceUpsertDto dto)
    {
        var existing = await _spaceRepository.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        existing.Name = dto.Name;
        existing.Code = dto.Code;
        existing.ImageUrl = dto.ImageUrl;
        existing.Description = dto.Description;
        existing.Location = dto.Location;
        existing.Floor = dto.Floor;
        existing.Building = dto.Building;
        existing.Capacity = dto.Capacity;
        existing.IsActive = dto.IsActive;
        existing.OpenTime = dto.OpenTime;
        existing.CloseTime = dto.CloseTime;
        existing.UpdatedAt = DateTime.UtcNow;

        await _spaceRepository.UpdateAsync(existing);
        return NoContent();
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var space = await _spaceRepository.GetByIdAsync(id);
        if (space is null) return NotFound();

        await _spaceRepository.DeleteAsync(space);
        return NoContent();
    }
}
