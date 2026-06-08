using GateWise.Core.Enums;

namespace GateWise.Core.DTOs;

public class CreateInviteDto
{
    public OrganizationMemberRole Role { get; set; } = OrganizationMemberRole.Member;
    public int? ExpiresInDays { get; set; }
    public int? MaxUses { get; set; }
}
