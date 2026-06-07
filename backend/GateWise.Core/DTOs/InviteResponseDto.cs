using GateWise.Core.Entities;
using GateWise.Core.Enums;

namespace GateWise.Core.DTOs;

public class InviteResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Email { get; set; }
    public OrganizationMemberRole Role { get; set; }
    public OrganizationInviteStatus Status { get; set; }
    public DateTime ExpiresAt { get; set; }

    public static InviteResponseDto From(OrganizationInvite invite) => new()
    {
        Id = invite.Id,
        Code = invite.Code,
        Email = invite.Email,
        Role = invite.Role,
        Status = invite.Status,
        ExpiresAt = invite.ExpiresAt
    };
}
