using GateWise.Core.Entities;
using GateWise.Core.Interfaces;
using GateWise.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GateWise.Infrastructure.Repositories;

public class OrganizationInviteRepository : IOrganizationInviteRepository
{
    private readonly AppDbContext _context;

    public OrganizationInviteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrganizationInvite?> GetByIdAsync(int id) =>
        await _context.OrganizationInvites.FindAsync(id);

    public async Task<OrganizationInvite?> GetByCodeAsync(string code) =>
        await _context.OrganizationInvites
            .Include(i => i.Organization)
            .Include(i => i.InviteSpaces).ThenInclude(s => s.Space)
            .FirstOrDefaultAsync(i => i.Code == code);

    public async Task<IEnumerable<OrganizationInvite>> GetByOrganizationIdAsync(int organizationId) =>
        await _context.OrganizationInvites
            .Include(i => i.InviteSpaces).ThenInclude(s => s.Space)
            .Where(i => i.OrganizationId == organizationId)
            .ToListAsync();

    public async Task AddAsync(OrganizationInvite invite)
    {
        _context.OrganizationInvites.Add(invite);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(OrganizationInvite invite)
    {
        _context.OrganizationInvites.Update(invite);
        await _context.SaveChangesAsync();
    }
}
