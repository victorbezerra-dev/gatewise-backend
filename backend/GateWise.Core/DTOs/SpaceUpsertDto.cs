namespace GateWise.Core.Dtos;

public class SpaceUpsertDto
{
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
}
