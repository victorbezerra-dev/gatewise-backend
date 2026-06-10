using GateWise.Core.Entities;
using GateWise.Core.Enums;

namespace GateWise.Core.DTOs;

public class InviteResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public OrganizationMemberRole Role { get; set; }
    public bool IsActive { get; set; }
    public int? MaxUses { get; set; }
    public int UsesCount { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? MemberStartsAt { get; set; }
    public DateTime? MemberExpiresAt { get; set; }
    public List<InviteSpaceDto> Spaces { get; set; } = [];
    public DateTime CreatedAt { get; set; }

    public static InviteResponseDto From(OrganizationInvite invite) => new()
    {
        Id = invite.Id,
        Code = invite.Code,
        Role = invite.Role,
        IsActive = invite.IsActive,
        MaxUses = invite.MaxUses,
        UsesCount = invite.UsesCount,
        ExpiresAt = invite.ExpiresAt,
        MemberStartsAt = invite.MemberStartsAt,
        MemberExpiresAt = invite.MemberExpiresAt,
        Spaces = invite.InviteSpaces.Select(s => new InviteSpaceDto { SpaceId = s.SpaceId, Name = s.Space.Name }).ToList(),
        CreatedAt = invite.CreatedAt
    };
}

public class InviteSpaceDto
{
    public int SpaceId { get; set; }
    public string Name { get; set; } = string.Empty;
}
