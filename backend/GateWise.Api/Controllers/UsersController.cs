using GateWise.Core.Entities;
using GateWise.Core.DTOs;
using GateWise.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GateWise.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UsersController(IUserRepository repository)
    {
        _repository = repository;
    }

    [Authorize()]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAll() =>
        Ok(await _repository.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(string id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [Authorize()]
    [HttpGet("me")]
    public async Task<ActionResult<User>> GetMe()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _repository.GetByIdAsync(userId);
        return user == null ? NotFound() : Ok(user);
    }

    [Authorize]
    [HttpPatch("me/public-key")]
    public async Task<IActionResult> UpdatePublicKey([FromBody] UpdatePublicKeyDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await _repository.UpdatePublicKeyAsync(userId, dto.DevicePublicKeyPem);
        return NoContent();
    }


    [Authorize()]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UserUpsertDto user)
    {
        await _repository.UpdateAsync(id, user);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
