using GateWise.Core.Entities;
using GateWise.Core.Interfaces;
using GateWise.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GateWise.Infrastructure.Repositories;

public class SpaceManagerRepository : ISpaceManagerRepository
{
    private readonly AppDbContext _context;

    public SpaceManagerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SpaceManager>> GetAllAsync()
    {
        return await _context.SpaceManagers
            .Include(s => s.Space)
            .Include(s => s.User)
            .ToListAsync();
    }

    public async Task<SpaceManager?> GetByIdAsync(int id)
    {
        return await _context.SpaceManagers
            .Include(s => s.Space)
            .Include(s => s.User)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<SpaceManager>> GetByUserIdAsync(string userId)
    {
        return await _context.SpaceManagers
            .Include(s => s.Space)
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> IsManagerOfSpaceAsync(int spaceId, string userId)
    {
        return await _context.SpaceManagers
            .AnyAsync(s => s.SpaceId == spaceId && s.UserId == userId);
    }

    public async Task AddAsync(SpaceManager spaceManager)
    {
        _context.SpaceManagers.Add(spaceManager);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(SpaceManager spaceManager)
    {
        _context.SpaceManagers.Remove(spaceManager);
        await _context.SaveChangesAsync();
    }
}
