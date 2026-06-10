using GateWise.Core.Enums;

namespace GateWise.Core.DTOs;

public class CreateInviteDto
{
    public OrganizationMemberRole Role { get; set; } = OrganizationMemberRole.Member;
    public int? ExpiresInDays { get; set; }
    public int? MaxUses { get; set; }
    public DateTime? MemberStartsAt { get; set; }
    public DateTime? MemberExpiresAt { get; set; }
    public List<int> SpaceIds { get; set; } = [];
}
