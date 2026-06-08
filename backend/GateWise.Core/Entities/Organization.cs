namespace GateWise.Core.Entities;

public class Organization
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Space> Spaces { get; set; } = [];
    public ICollection<OrganizationMember> Members { get; set; } = [];
    public ICollection<OrganizationInvite> Invites { get; set; } = [];
}
