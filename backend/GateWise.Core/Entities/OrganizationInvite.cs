using GateWise.Core.Enums;

namespace GateWise.Core.Entities;

public class OrganizationInvite
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public string Code { get; set; } = string.Empty;
    public OrganizationMemberRole Role { get; set; } = OrganizationMemberRole.Member;
    public bool IsActive { get; set; } = true;
    public int? MaxUses { get; set; }
    public int UsesCount { get; set; } = 0;
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
