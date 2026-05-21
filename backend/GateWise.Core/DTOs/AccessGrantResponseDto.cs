using GateWise.Core.Enums;

namespace GateWise.Application.DTOs;

public class AccessGrantResponseDto
{
    public int Id { get; set; }
    public required string AuthorizedUserId { get; set; }
    public string AuthorizedUserName { get; set; } = string.Empty;

    public required string GrantedByUserId { get; set; }
    public string GrantedByUserName { get; set; } = string.Empty;

    public int SpaceId { get; set; }
    public string SpaceName { get; set; } = string.Empty;

    public DateTime? GrantedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public string Reason { get; set; } = string.Empty;
    public AccessGrantStatus Status { get; set; }
}
