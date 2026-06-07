using GateWise.Core.Entities;

namespace GateWise.Core.DTOs;

public class SpaceManagerResponseDto
{
    public int Id { get; set; }
    public int SpaceId { get; set; }
    public required string UserId { get; set; }
    public string? SpaceName { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }

    public static SpaceManagerResponseDto From(SpaceManager manager) => new()
    {
        Id = manager.Id,
        SpaceId = manager.SpaceId,
        UserId = manager.UserId,
        SpaceName = manager.Space?.Name,
        UserName = manager.User?.Name,
        UserEmail = manager.User?.Email
    };
}