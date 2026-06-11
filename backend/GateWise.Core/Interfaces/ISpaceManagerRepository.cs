using GateWise.Core.Entities;

namespace GateWise.Core.Interfaces;

public interface ISpaceManagerRepository
{
    Task<IEnumerable<SpaceManager>> GetAllAsync();
    Task<SpaceManager?> GetByIdAsync(int id);
    Task<IEnumerable<SpaceManager>> GetByUserIdAsync(string userId);
    Task<IEnumerable<SpaceManager>> GetByOrganizationIdAsync(int organizationId);
    Task<bool> IsManagerOfSpaceAsync(int spaceId, string userId);
    Task AddAsync(SpaceManager spaceManager);
    Task DeleteAsync(SpaceManager spaceManager);
}
