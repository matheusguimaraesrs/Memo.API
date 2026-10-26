using Memo.Domain.Common;

namespace Memo.Infra.Services;

public interface IPaginationService
{
    Task<PagedList<T>> PaginateAsync<T>(
        IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken ct = default);
}