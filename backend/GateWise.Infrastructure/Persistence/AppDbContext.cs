using Microsoft.EntityFrameworkCore;
using GateWise.Core.Entities;

namespace GateWise.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationMember> OrganizationMembers => Set<OrganizationMember>();
    public DbSet<OrganizationInvite> OrganizationInvites => Set<OrganizationInvite>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Space> Spaces => Set<Space>();
    public DbSet<SpaceManager> SpaceManagers => Set<SpaceManager>();
    public DbSet<AccessGrant> AccessGrants => Set<AccessGrant>();
    public DbSet<AccessLog> AccessLogs => Set<AccessLog>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSnakeCaseNamingConvention();
}
