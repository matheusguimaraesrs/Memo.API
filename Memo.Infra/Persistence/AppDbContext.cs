using System.Linq.Expressions;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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
}
