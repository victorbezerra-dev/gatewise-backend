using GateWise.Core.Enums;

namespace GateWise.Core.Entities;

public class OrganizationInvite
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public string? Email { get; set; }
    public string Code { get; set; } = string.Empty;
    public OrganizationMemberRole Role { get; set; } = OrganizationMemberRole.Member;
    public OrganizationInviteStatus Status { get; set; } = OrganizationInviteStatus.Pending;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
