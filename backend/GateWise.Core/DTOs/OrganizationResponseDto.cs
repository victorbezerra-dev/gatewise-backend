using GateWise.Core.Entities;

namespace GateWise.Core.DTOs;

public class OrganizationResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static OrganizationResponseDto From(Organization org) => new()
    {
        Id = org.Id,
        Name = org.Name,
        Description = org.Description,
        IsActive = org.IsActive,
        CreatedAt = org.CreatedAt,
        UpdatedAt = org.UpdatedAt
    };
}
