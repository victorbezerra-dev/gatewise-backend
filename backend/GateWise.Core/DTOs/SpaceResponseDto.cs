using GateWise.Core.Entities;

namespace GateWise.Core.DTOs;

public class SpaceResponseDto
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Floor { get; set; }
    public string Building { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
    public DateTime OpenTime { get; set; }
    public DateTime CloseTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static SpaceResponseDto From(Space space) => new()
    {
        Id = space.Id,
        OrganizationId = space.OrganizationId,
        Name = space.Name,
        Code = space.Code,
        ImageUrl = space.ImageUrl,
        Description = space.Description,
        Location = space.Location,
        Floor = space.Floor,
        Building = space.Building,
        Capacity = space.Capacity,
        IsActive = space.IsActive,
        OpenTime = space.OpenTime,
        CloseTime = space.CloseTime,
        CreatedAt = space.CreatedAt,
        UpdatedAt = space.UpdatedAt
    };
}