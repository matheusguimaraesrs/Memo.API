using Memo.Domain.Common;
using Memo.Infra.Persistence;

namespace Memo.Infra.Services;

internal sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return context.SaveChangesAsync(ct);
    }
}