using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GateWise.Core.Entities;
using GateWise.Core.Enums;

public class AccessLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string CommandId { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public required string UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = default!;

    [Required]
    public int SpaceId { get; set; }

    [ForeignKey(nameof(SpaceId))]
    public Space Space { get; set; } = default!;

    [Required]
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ConfirmedAt { get; set; }

    [Required]
    public AccessStatus Status { get; set; } = AccessStatus.PENDING_CONFIRMATION;

    [Required]
    public string RawRequestJson { get; set; } = "{}";

    public string? RawConfirmationJson { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
