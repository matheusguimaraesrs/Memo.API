using System.Linq.Expressions;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Persistence;

public sealed class AppDbContext : DbContext
{
    private readonly IUserContext _userContext;
    public AppDbContext(DbContextOptions<AppDbContext> options, IUserContext userContext) : base(options)
    {
        _userContext = userContext;
    }

    public DbSet<Seller> Sellers => Set<Seller>();
    public DbSet<RepresentedCompany> RepresentedCompanies => Set<RepresentedCompany>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(AuditableEntity).IsAssignableFrom(entity.ClrType))
            {
                var parameter = Expression.Parameter(entity.ClrType, "e");
                var property = Expression.Property(parameter, nameof(AuditableEntity.IsDeleted));
                var falseConstant = Expression.Constant(false);
                var comparison = Expression.Equal(property, falseConstant);
                var lambda = Expression.Lambda(comparison, parameter);

                modelBuilder.Entity(entity.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.WhoCreated(_userContext.Name);
                    break;
                case EntityState.Modified:
                    entry.Property(nameof(AuditableEntity.CreatedAt)).IsModified = false;
                    entry.Property(nameof(AuditableEntity.CreatedBy)).IsModified = false;
                    entry.Entity.UpdateAudit(_userContext.Name);
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Property(nameof(AuditableEntity.CreatedAt)).IsModified = false;
                    entry.Property(nameof(AuditableEntity.CreatedBy)).IsModified = false;
                    entry.Entity.SoftDelete(_userContext.Name);
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
