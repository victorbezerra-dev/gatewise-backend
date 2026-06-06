using GateWise.Core.Enums;

namespace GateWise.Core.DTOs;

public class CreateInviteDto
{
    public string? Email { get; set; }
    public OrganizationMemberRole Role { get; set; } = OrganizationMemberRole.Member;
    public int ExpiresInDays { get; set; } = 7;
}
