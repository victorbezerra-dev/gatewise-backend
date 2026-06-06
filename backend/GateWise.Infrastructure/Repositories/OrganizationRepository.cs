using GateWise.Core.Entities;
using GateWise.Core.Interfaces;
using GateWise.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GateWise.Infrastructure.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly AppDbContext _context;

    public OrganizationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Organization>> GetAllAsync() =>
        await _context.Organizations.ToListAsync();

    public async Task<Organization?> GetByIdAsync(int id) =>
        await _context.Organizations.FindAsync(id);

    public async Task<Organization> AddAsync(Organization organization)
    {
        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync();
        return organization;
    }

    public async Task UpdateAsync(Organization organization)
    {
        organization.UpdatedAt = DateTime.UtcNow;
        _context.Organizations.Update(organization);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Organization organization)
    {
        _context.Organizations.Remove(organization);
        await _context.SaveChangesAsync();
    }
}
