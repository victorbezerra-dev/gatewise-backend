namespace GateWise.Application.DTOs;

public class CreateAccessGrantDto
{
    public int SpaceId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
