using GateWise.Core.Entities;
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

    public static UserMeResponseDto From(User user, IEnumerable<OrganizationMember> memberships) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        RegistrationNumber = user.RegistrationNumber,
        UserAvatarUrl = user.UserAvatarUrl,
        UserType = user.UserType,
        DevicePublicKeyPem = user.DevicePublicKeyPem,
        Organizations = memberships.Select(m => new UserOrganizationDto
        {
            Id = m.OrganizationId,
            Name = m.Organization.Name,
            Role = m.Role
        }).ToList()
    };
}

public class UserOrganizationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public OrganizationMemberRole Role { get; set; }
}
