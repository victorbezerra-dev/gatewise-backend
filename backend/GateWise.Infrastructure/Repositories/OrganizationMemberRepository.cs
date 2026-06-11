using GateWise.Core.Entities;
using GateWise.Core.Enums;
using GateWise.Core.Interfaces;
using GateWise.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GateWise.Infrastructure.Repositories;

public class OrganizationMemberRepository : IOrganizationMemberRepository
{
    private readonly AppDbContext _context;

    public OrganizationMemberRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrganizationMember>> GetByOrganizationIdAsync(int organizationId) =>
        await _context.OrganizationMembers
            .Include(m => m.User)
            .Where(m => m.OrganizationId == organizationId)
            .ToListAsync();

    public async Task<IEnumerable<OrganizationMember>> GetByUserIdAsync(string userId) =>
        await _context.OrganizationMembers
            .Include(m => m.Organization)
            .Where(m => m.UserId == userId)
            .ToListAsync();

    public async Task<OrganizationMember?> GetByIdAsync(int id) =>
        await _context.OrganizationMembers
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<OrganizationMember?> GetAsync(int organizationId, string userId) =>
        await _context.OrganizationMembers
            .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.UserId == userId);

    public async Task<bool> IsOwnerOrManagerAsync(int organizationId, string userId) =>
        await _context.OrganizationMembers
            .AnyAsync(m =>
                m.OrganizationId == organizationId &&
                m.UserId == userId &&
                (m.Role == OrganizationMemberRole.Owner || m.Role == OrganizationMemberRole.Manager));

    public async Task AddAsync(OrganizationMember member)
    {
        _context.OrganizationMembers.Add(member);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(OrganizationMember member)
    {
        _context.OrganizationMembers.Update(member);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRoleWithSpacesAsync(OrganizationMember member, IEnumerable<int>? newSpaceIds)
    {
        _context.OrganizationMembers.Update(member);

        var existingManagers = await _context.SpaceManagers
            .Include(sm => sm.Space)
            .Where(sm => sm.UserId == member.UserId && sm.Space.OrganizationId == member.OrganizationId)
            .ToListAsync();

        _context.SpaceManagers.RemoveRange(existingManagers);

        if (member.Role == OrganizationMemberRole.Manager && newSpaceIds is not null)
        {
            foreach (var spaceId in newSpaceIds)
                _context.SpaceManagers.Add(new SpaceManager { SpaceId = spaceId, UserId = member.UserId });
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(OrganizationMember member)
    {
        _context.OrganizationMembers.Remove(member);
        await _context.SaveChangesAsync();
    }
}
