using GateWise.Core.Entities;

namespace GateWise.Core.Interfaces;

public interface ISpaceManagerRepository
{
    Task<IEnumerable<SpaceManager>> GetAllAsync();
    Task<SpaceManager?> GetByIdAsync(int id);
    Task AddAsync(SpaceManager spaceManager);
    Task DeleteAsync(SpaceManager spaceManager);
}
