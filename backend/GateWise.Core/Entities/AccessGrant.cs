using GateWise.Core.Enums;

namespace GateWise.Core.Entities;

public class AccessGrant
{
    public int Id { get; set; }

    public required string AuthorizedUserId { get; set; }
    public User AuthorizedUser { get; set; } = null!;

    public string? GrantedByUserId { get; set; }
    public User? GrantedByUser { get; set; }

    public int SpaceId { get; set; }
    public Space Space { get; set; } = null!;

    public DateTime? GrantedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public string Reason { get; set; } = string.Empty;

    public AccessGrantStatus Status { get; set; } = AccessGrantStatus.Pending;
}
