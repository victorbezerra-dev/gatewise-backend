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
    private readonly IOrganizationMemberRepository _memberRepository;

    public UsersController(
        IUserRepository repository,
        IOrganizationMemberRepository memberRepository)
    {
        _repository = repository;
        _memberRepository = memberRepository;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
    {
        var users = await _repository.GetAllAsync();
        return Ok(users.Select(UserResponseDto.From));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetById(string id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(UserResponseDto.From(user));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserMeResponseDto>> GetMe()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var name = User.FindFirst("name")?.Value
            ?? User.FindFirst("preferred_username")?.Value
            ?? string.Empty;
        var email = User.FindFirst(ClaimTypes.Email)?.Value
            ?? User.FindFirst("email")?.Value
            ?? string.Empty;

        var user = await _repository.UpsertFromClaimsAsync(userId, name, email);

        var memberships = await _memberRepository.GetByUserIdAsync(userId);

        return Ok(UserMeResponseDto.From(user, memberships));
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

    [Authorize]
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
