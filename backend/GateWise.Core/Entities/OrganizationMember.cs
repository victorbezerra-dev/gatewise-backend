using GateWise.Core.Enums;

namespace GateWise.Core.Entities;

public class OrganizationMember
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public required string UserId { get; set; }
    public User User { get; set; } = null!;
    public OrganizationMemberRole Role { get; set; } = OrganizationMemberRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
