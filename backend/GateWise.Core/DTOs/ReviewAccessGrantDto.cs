using GateWise.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace GateWise.Application.DTOs;

public class ReviewAccessGrantDto
{
    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(AccessGrantStatus))]
    public AccessGrantStatus Status { get; set; }
    public string? Reason { get; set; }
}
