using GateWise.Core.Enums;

namespace GateWise.Core.DTOs;

public class UpdateMemberRoleDto
{
    public OrganizationMemberRole Role { get; set; }
    public List<int>? SpaceIds { get; set; }
}
