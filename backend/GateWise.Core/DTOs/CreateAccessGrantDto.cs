namespace GateWise.Application.DTOs;

public class CreateAccessGrantDto
{
    public int LabId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
