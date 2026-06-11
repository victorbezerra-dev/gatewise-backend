using GateWise.Core.Entities;

namespace GateWise.Core.Interfaces;

public interface IOrganizationInviteRepository
{
    Task<OrganizationInvite?> GetByIdAsync(int id);
    Task<OrganizationInvite?> GetByCodeAsync(string code);
    Task<IEnumerable<OrganizationInvite>> GetByOrganizationIdAsync(int organizationId);
    Task AddAsync(OrganizationInvite invite);
    Task UpdateAsync(OrganizationInvite invite);
    Task RemoveSpacesAsync(int inviteId, IEnumerable<int> spaceIds);
}
