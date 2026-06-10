namespace GateWise.Core.Entities;

public class OrganizationInviteSpace
{
    public int Id { get; set; }

    public int InviteId { get; set; }
    public OrganizationInvite Invite { get; set; } = null!;

    public int SpaceId { get; set; }
    public Space Space { get; set; } = null!;
}
