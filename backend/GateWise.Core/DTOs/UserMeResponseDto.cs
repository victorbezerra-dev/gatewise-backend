using GateWise.Core.Enums;

namespace GateWise.Core.DTOs;

public class UserMeResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string UserAvatarUrl { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public string? DevicePublicKeyPem { get; set; }
    public List<UserOrganizationDto> Organizations { get; set; } = [];
}

public class UserOrganizationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public OrganizationMemberRole Role { get; set; }
}
