using GateWise.Core.Entities;

namespace GateWise.Core.Interfaces;

public interface ISpaceRepository
{
    Task<IEnumerable<Space>> GetAllAsync();
    Task<IEnumerable<Space>> GetByOrganizationIdAsync(int organizationId);
    Task<Space?> GetByIdAsync(int id);
    Task AddAsync(Space space);
    Task UpdateAsync(Space space);
    Task DeleteAsync(Space space);
}
