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
        var userName = _userContext?.UserName;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.WhoCreated(userName);
                    break;
                case EntityState.Modified:
                    entry.Property(p => p.CreatedAt).IsModified = false;
                    entry.Property(p => p.CreatedBy).IsModified = false;
                    entry.Entity.Update(userName);
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.SoftDelete(userName);
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
