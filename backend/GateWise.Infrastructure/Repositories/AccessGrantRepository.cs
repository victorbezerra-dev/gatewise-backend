using GateWise.Core.Entities;
using GateWise.Core.Enums;
using GateWise.Core.Interfaces;
using GateWise.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GateWise.Infrastructure.Repositories;

public class AccessGrantRepository : IAccessGrantRepository
{
    private readonly AppDbContext _context;

    public AccessGrantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AccessGrant>> GetAllAsync(string? search = null)
    {
        var query = _context.AccessGrants
            .Include(g => g.AuthorizedUser)
            .Include(g => g.GrantedByUser)
            .Include(g => g.Space)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(grant =>
                grant.AuthorizedUser.Name.ToLower().Contains(search) ||
                grant.AuthorizedUser.Email.ToLower().Contains(search) ||
                grant.AuthorizedUser.RegistrationNumber.ToString().Contains(search)
            );
        }

        return await query.ToListAsync();
    }


    public async Task<AccessGrant?> GetByIdAsync(int id)
    {
        return await _context.AccessGrants
            .Include(g => g.AuthorizedUser)
            .Include(g => g.GrantedByUser)
            .Include(g => g.Space)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<AccessGrant>> GetByUserIdAsync(string userId)
    {
        return await _context.AccessGrants
            .Include(g => g.AuthorizedUser)
            .Include(g => g.GrantedByUser)
            .Include(g => g.Space)
            .Where(g => g.AuthorizedUserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<AccessGrant>> GetByOrganizationIdAsync(int organizationId)
    {
        return await _context.AccessGrants
            .Include(g => g.Space)
            .Where(g => g.Space.OrganizationId == organizationId && g.Status == AccessGrantStatus.Granted)
            .ToListAsync();
    }

    public async Task AddAsync(AccessGrant grant)
    {
        _context.AccessGrants.Add(grant);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AccessGrant grant)
    {
        _context.AccessGrants.Update(grant);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(AccessGrant grant)
    {
        _context.AccessGrants.Remove(grant);
        await _context.SaveChangesAsync();
    }

}
