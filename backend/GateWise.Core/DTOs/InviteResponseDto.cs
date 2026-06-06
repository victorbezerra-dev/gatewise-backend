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
}
