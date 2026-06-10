using System.Security.Claims;
using System.Security.Cryptography;
using GateWise.Core.DTOs;
using GateWise.Core.Entities;
using GateWise.Core.Enums;
using GateWise.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GateWise.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IOrganizationMemberRepository _memberRepositorysitory;
    private readonly IOrganizationInviteRepository _inviteRepositorysitory;
    private readonly ISpaceManagerRepository _spaceManagerRepository;
    private readonly IAccessGrantRepository _accessGrantRepository;
    private readonly ISpaceRepository _spaceRepository;

    public OrganizationsController(
        IOrganizationRepository organizationRepository,
        IOrganizationMemberRepository memberRepository,
        IOrganizationInviteRepository inviteRepository,
        ISpaceManagerRepository spaceManagerRepository,
        IAccessGrantRepository accessGrantRepository,
        ISpaceRepository spaceRepository)
    {
        _organizationRepository = organizationRepository;
        _memberRepositorysitory = memberRepository;
        _inviteRepositorysitory = inviteRepository;
        _spaceManagerRepository = spaceManagerRepository;
        _accessGrantRepository = accessGrantRepository;
        _spaceRepository = spaceRepository;
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<IEnumerable<OrganizationResponseDto>>> GetAll()
    {
        var orgs = await _organizationRepository.GetAllAsync();
        return Ok(orgs.Select(OrganizationResponseDto.From));
    }

    [HttpGet("memberships")]
    public async Task<IActionResult> GetMemberships()
    {
        var userId = GetUserId();
        var memberships = await _memberRepositorysitory.GetByUserIdAsync(userId);
        var result = memberships.Select(m => new
        {
            Organization = OrganizationResponseDto.From(m.Organization),
            m.Role,
            m.JoinedAt,
            m.StartsAt,
            m.ExpiresAt
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserId();
        var org = await _organizationRepository.GetByIdAsync(id);
        if (org is null) return NotFound();

        OrganizationMember? member = null;
        if (!User.IsInRole("admin"))
        {
            member = await _memberRepositorysitory.GetAsync(id, userId);
            if (member is null) return Forbid();
        }

        List<object> managedSpaces;

        if (User.IsInRole("admin") || member?.Role == OrganizationMemberRole.Owner)
        {
            var allSpaces = await _spaceRepository.GetByOrganizationIdAsync(id);
            managedSpaces = allSpaces.Select(s => (object)new { spaceId = s.Id, name = s.Name }).ToList();
        }
        else if (member?.Role == OrganizationMemberRole.Manager)
        {
            managedSpaces = (await _spaceManagerRepository.GetByUserIdAsync(userId))
                .Where(sm => sm.Space.OrganizationId == id)
                .Select(sm => (object)new { spaceId = sm.SpaceId, name = sm.Space.Name })
                .ToList();
        }
        else
        {
            managedSpaces = [];
        }

        return Ok(new
        {
            Organization = OrganizationResponseDto.From(org),
            member?.Role,
            member?.JoinedAt,
            member?.StartsAt,
            member?.ExpiresAt,
            ManagedSpaces = managedSpaces
        });
    }

    [HttpPost]
    public async Task<ActionResult<OrganizationResponseDto>> Create([FromBody] OrganizationUpsertDto dto)
    {
        var userId = GetUserId();

        var org = new Organization
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _organizationRepository.AddAsync(org);

        var membership = new OrganizationMember
        {
            OrganizationId = org.Id,
            UserId = userId,
            Role = OrganizationMemberRole.Owner,
            JoinedAt = DateTime.UtcNow
        };

        await _memberRepositorysitory.AddAsync(membership);

        return CreatedAtAction(nameof(GetById), new { id = org.Id }, OrganizationResponseDto.From(org));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] OrganizationUpsertDto dto)
    {
        var userId = GetUserId();
        var org = await _organizationRepository.GetByIdAsync(id);
        if (org is null) return NotFound();

        if (!User.IsInRole("admin"))
        {
            var member = await _memberRepositorysitory.GetAsync(id, userId);
            if (member is null || member.Role != OrganizationMemberRole.Owner)
                return Forbid();
        }

        org.Name = dto.Name;
        org.Description = dto.Description;
        org.IsActive = dto.IsActive;

        await _organizationRepository.UpdateAsync(org);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var org = await _organizationRepository.GetByIdAsync(id);
        if (org is null) return NotFound();

        if (!User.IsInRole("admin"))
        {
            var member = await _memberRepositorysitory.GetAsync(id, userId);
            if (member is null || member.Role != OrganizationMemberRole.Owner)
                return Forbid();
        }

        await _organizationRepository.DeleteAsync(org);
        return NoContent();
    }

    [HttpPost("{id}/invites")]
    public async Task<ActionResult<InviteResponseDto>> CreateInvite(int id, [FromBody] CreateInviteDto dto)
    {
        var userId = GetUserId();

        if (!User.IsInRole("admin"))
        {
            var caller = await _memberRepositorysitory.GetAsync(id, userId);
            if (caller is null) return Forbid();

            if (caller.Role == OrganizationMemberRole.Manager)
            {
                if (dto.Role != OrganizationMemberRole.Member)
                    return Forbid();

                foreach (var spaceId in dto.SpaceIds)
                {
                    if (!await _spaceManagerRepository.IsManagerOfSpaceAsync(spaceId, userId))
                        return BadRequest($"You are not a manager of space {spaceId}.");
                }
            }
            else if (caller.Role != OrganizationMemberRole.Owner)
            {
                return Forbid();
            }
        }

        var org = await _organizationRepository.GetByIdAsync(id);
        if (org is null) return NotFound();

        var invite = new OrganizationInvite
        {
            OrganizationId = id,
            Code = Convert.ToHexString(RandomNumberGenerator.GetBytes(4)),
            Role = dto.Role,
            IsActive = true,
            MaxUses = dto.MaxUses,
            UsesCount = 0,
            ExpiresAt = dto.ExpiresInDays.HasValue ? DateTime.UtcNow.AddDays(dto.ExpiresInDays.Value) : null,
            MemberStartsAt = dto.MemberStartsAt,
            MemberExpiresAt = dto.MemberExpiresAt,
            CreatedAt = DateTime.UtcNow,
            InviteSpaces = dto.Role == OrganizationMemberRole.Manager
                ? dto.SpaceIds.Select(spaceId => new OrganizationInviteSpace { SpaceId = spaceId }).ToList()
                : []
        };

        await _inviteRepositorysitory.AddAsync(invite);

        return Ok(InviteResponseDto.From(invite));
    }

    [HttpGet("{id}/invites")]
    public async Task<ActionResult<IEnumerable<InviteResponseDto>>> GetInvites(int id)
    {
        var userId = GetUserId();

        if (!User.IsInRole("admin"))
        {
            var caller = await _memberRepositorysitory.GetAsync(id, userId);
            if (caller is null || caller.Role != OrganizationMemberRole.Owner)
                return Forbid();
        }

        var invites = await _inviteRepositorysitory.GetByOrganizationIdAsync(id);
        return Ok(invites.Select(InviteResponseDto.From));
    }

    [HttpPost("join")]
    public async Task<ActionResult<OrganizationResponseDto>> Join([FromBody] JoinOrganizationDto dto)
    {
        var userId = GetUserId();

        var invite = await _inviteRepositorysitory.GetByCodeAsync(dto.Code);
        if (invite is null || !invite.IsActive)
            return BadRequest("Invalid or expired invite code.");

        if (invite.ExpiresAt.HasValue && invite.ExpiresAt.Value < DateTime.UtcNow)
            return BadRequest("Invalid or expired invite code.");

        if (invite.MaxUses.HasValue && invite.UsesCount >= invite.MaxUses.Value)
            return BadRequest("Invalid or expired invite code.");

        var existing = await _memberRepositorysitory.GetAsync(invite.OrganizationId, userId);
        if (existing is not null)
            return Conflict("You are already a member of this organization.");

        var membership = new OrganizationMember
        {
            OrganizationId = invite.OrganizationId,
            UserId = userId,
            Role = invite.Role,
            JoinedAt = DateTime.UtcNow,
            StartsAt = invite.MemberStartsAt,
            ExpiresAt = invite.MemberExpiresAt
        };

        await _memberRepositorysitory.AddAsync(membership);

        if (invite.Role == OrganizationMemberRole.Manager)
        {
            foreach (var inviteSpace in invite.InviteSpaces)
            {
                await _spaceManagerRepository.AddAsync(new SpaceManager
                {
                    SpaceId = inviteSpace.SpaceId,
                    UserId = userId
                });
            }
        }
        else if (invite.Role == OrganizationMemberRole.Member)
        {
            foreach (var inviteSpace in invite.InviteSpaces)
            {
                await _accessGrantRepository.AddAsync(new AccessGrant
                {
                    AuthorizedUserId = userId,
                    SpaceId = inviteSpace.SpaceId,
                    Status = AccessGrantStatus.Granted,
                    GrantedAt = DateTime.UtcNow,
                    Reason = "Granted via invite"
                });
            }
        }

        invite.UsesCount++;
        if (invite.MaxUses.HasValue && invite.UsesCount >= invite.MaxUses.Value)
            invite.IsActive = false;

        await _inviteRepositorysitory.UpdateAsync(invite);

        return Ok(OrganizationResponseDto.From(invite.Organization));
    }

    [HttpGet("{id}/members")]
    public async Task<IActionResult> GetMembers(int id)
    {
        var userId = GetUserId();
        if (!User.IsInRole("admin") && !await _memberRepositorysitory.IsOwnerOrManagerAsync(id, userId))
            return Forbid();

        var members = await _memberRepositorysitory.GetByOrganizationIdAsync(id);
        return Ok(members.Select(m => new
        {
            m.Id,
            m.UserId,
            m.User.Name,
            m.User.Email,
            m.Role,
            m.JoinedAt,
            m.StartsAt,
            m.ExpiresAt
        }));
    }

    [HttpDelete("{id}/members/{memberId}")]
    public async Task<IActionResult> RemoveMember(int id, int memberId)
    {
        var userId = GetUserId();
        if (!User.IsInRole("admin"))
        {
            var caller = await _memberRepositorysitory.GetAsync(id, userId);
            if (caller is null || caller.Role != OrganizationMemberRole.Owner)
                return Forbid();
        }

        var member = await _memberRepositorysitory.GetByIdAsync(memberId);
        if (member is null || member.OrganizationId != id)
            return NotFound();

        if (member.Role == OrganizationMemberRole.Owner)
            return BadRequest("Cannot remove the organization owner.");

        await _memberRepositorysitory.DeleteAsync(member);
        return NoContent();
    }

    [HttpDelete("{id}/invites/{inviteId}")]
    public async Task<IActionResult> RevokeInvite(int id, int inviteId)
    {
        var userId = GetUserId();
        if (!User.IsInRole("admin"))
        {
            var caller = await _memberRepositorysitory.GetAsync(id, userId);
            if (caller is null || caller.Role != OrganizationMemberRole.Owner)
                return Forbid();
        }

        var invite = await _inviteRepositorysitory.GetByIdAsync(inviteId);
        if (invite is null || invite.OrganizationId != id)
            return NotFound();

        invite.IsActive = false;
        await _inviteRepositorysitory.UpdateAsync(invite);
        return NoContent();
    }

    private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();

}
