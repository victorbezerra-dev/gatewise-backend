using GateWise.Application.DTOs;
using GateWise.Core.DTOs;
using GateWise.Core.Entities;
using GateWise.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GateWise.Api.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/[controller]")]
public class SpaceManagersController : ControllerBase
{
    private readonly ISpaceManagerRepository _spaceManagerRepo;
    private readonly ISpaceRepository _spaceRepo;
    private readonly IUserRepository _userRepo;

    public SpaceManagersController(
        ISpaceManagerRepository spaceManagerRepo,
        ISpaceRepository spaceRepo,
        IUserRepository userRepo)
    {
        _spaceManagerRepo = spaceManagerRepo;
        _spaceRepo = spaceRepo;
        _userRepo = userRepo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpaceManagerResponseDto>>> GetAll()
    {
        var result = await _spaceManagerRepo.GetAllAsync();
        return Ok(result.Select(SpaceManagerResponseDto.From));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SpaceManagerResponseDto>> GetById(int id)
    {
        var manager = await _spaceManagerRepo.GetByIdAsync(id);
        if (manager is null)
            return NotFound();

        return Ok(SpaceManagerResponseDto.From(manager));
    }

    [HttpPost]
    public async Task<ActionResult<SpaceManagerResponseDto>> Create([FromBody] CreateSpaceManagerDto input)
    {
        var space = await _spaceRepo.GetByIdAsync(input.SpaceId);
        var user = await _userRepo.GetByIdAsync(input.UserId);

        if (space is null || user is null)
            return BadRequest("Invalid SpaceId or UserId.");

        var newManager = new SpaceManager
        {
            SpaceId = input.SpaceId,
            UserId = input.UserId
        };

        await _spaceManagerRepo.AddAsync(newManager);
        return CreatedAtAction(nameof(GetById), new { id = newManager.Id }, SpaceManagerResponseDto.From(newManager));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var manager = await _spaceManagerRepo.GetByIdAsync(id);
        if (manager is null)
            return NotFound();

        await _spaceManagerRepo.DeleteAsync(manager);
        return NoContent();
    }

}
