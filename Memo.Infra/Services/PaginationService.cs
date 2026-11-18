using Memo.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Services;

public sealed class PaginationService : IPaginationService
{
    public async Task<PagedList<T>> PaginateAsync<T>(
        IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        int totalItems = await query.CountAsync(ct);

        List<T> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedList<T>(items, page, pageSize, totalItems);
    }
}