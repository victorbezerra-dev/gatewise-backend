using GateWise.Core.Entities;
using GateWise.Core.Enums;

namespace GateWise.Core.Interfaces;

public interface IOrganizationMemberRepository
{
    Task<IEnumerable<OrganizationMember>> GetByOrganizationIdAsync(int organizationId);
    Task<IEnumerable<OrganizationMember>> GetByUserIdAsync(string userId);
    Task<OrganizationMember?> GetByIdAsync(int id);
    Task<OrganizationMember?> GetAsync(int organizationId, string userId);
    Task<bool> IsOwnerOrManagerAsync(int organizationId, string userId);
    Task AddAsync(OrganizationMember member);
    Task UpdateAsync(OrganizationMember member);
    Task DeleteAsync(OrganizationMember member);
}
