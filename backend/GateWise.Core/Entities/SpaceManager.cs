namespace GateWise.Core.Entities;

public class SpaceManager
{
    public int Id { get; set; }

    public int SpaceId { get; set; }
    public Space Space { get; set; } = null!;

    public required string UserId { get; set; }
    public User User { get; set; } = null!;
}
