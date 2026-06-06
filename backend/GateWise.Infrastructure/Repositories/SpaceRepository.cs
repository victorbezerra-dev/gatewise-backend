using GateWise.Core.Entities;
using GateWise.Core.Interfaces;
using GateWise.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GateWise.Infrastructure.Repositories;

public class SpaceRepository : ISpaceRepository
{
    private readonly AppDbContext _context;

    public SpaceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Space>> GetAllAsync() =>
        await _context.Spaces.ToListAsync();

    public async Task<IEnumerable<Space>> GetByOrganizationIdAsync(int organizationId) =>
        await _context.Spaces.Where(s => s.OrganizationId == organizationId).ToListAsync();

    public async Task<Space?> GetByIdAsync(int id)
    {
        return await _context.Spaces.FindAsync(id);
    }

    public async Task AddAsync(Space space)
    {
        space.CreatedAt = DateTime.UtcNow;
        space.UpdatedAt = DateTime.UtcNow;
        _context.Spaces.Add(space);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Space space)
    {
        space.UpdatedAt = DateTime.UtcNow;
        _context.Spaces.Update(space);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Space space)
    {
        _context.Spaces.Remove(space);
        await _context.SaveChangesAsync();
    }
}
