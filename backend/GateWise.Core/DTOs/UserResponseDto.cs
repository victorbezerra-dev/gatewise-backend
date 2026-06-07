using GateWise.Core.Entities;
using GateWise.Core.Enums;

namespace GateWise.Core.DTOs;

public class UserResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string UserAvatarUrl { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public string OperationalSystem { get; set; } = string.Empty;
    public string OperationalSystemVersion { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public string DeviceManufactureName { get; set; } = string.Empty;
    public string? DevicePublicKeyPem { get; set; }

    public static UserResponseDto From(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        RegistrationNumber = user.RegistrationNumber,
        UserAvatarUrl = user.UserAvatarUrl,
        UserType = user.UserType,
        OperationalSystem = user.OperationalSystem,
        OperationalSystemVersion = user.OperationalSystemVersion,
        DeviceModel = user.DeviceModel,
        DeviceManufactureName = user.DeviceManufactureName,
        DevicePublicKeyPem = user.DevicePublicKeyPem
    };
}