using GateWise.Core.Enums;

namespace GateWise.Core.Entities;

public class User
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

    public ICollection<OrganizationMember> OrganizationMemberships { get; set; } = [];

    private User() { }

    public User(
        string name,
        string email,
        string registration,
        string userAvatarUrl,
        UserType userType,
        string operationalSystem,
        string operationalSystemVersion,
        string deviceModel,
        string deviceManufactureName,
        string? devicePublicKeyPem = null)
    {
        Name = name;
        Email = email;
        RegistrationNumber = registration;
        UserAvatarUrl = userAvatarUrl;
        UserType = userType;
        OperationalSystem = operationalSystem;
        OperationalSystemVersion = operationalSystemVersion;
        DeviceModel = deviceModel;
        DeviceManufactureName = deviceManufactureName;
        DevicePublicKeyPem = devicePublicKeyPem;
    }
}
