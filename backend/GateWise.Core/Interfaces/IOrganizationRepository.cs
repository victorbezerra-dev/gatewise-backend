using GateWise.Core.Entities;

namespace GateWise.Core.Interfaces;

public interface IOrganizationRepository
{
    Task<IEnumerable<Organization>> GetAllAsync();
    Task<Organization?> GetByIdAsync(int id);
    Task<Organization> AddAsync(Organization organization);
    Task UpdateAsync(Organization organization);
    Task DeleteAsync(Organization organization);
}
