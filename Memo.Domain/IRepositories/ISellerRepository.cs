using Memo.Domain.Common;
using Memo.Domain.Entities;

namespace Memo.Domain.IRepositories;

public interface ISellerRepository
{
    Task<PagedList<Seller>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<Seller?> GetByIdAsync(Guid id, CancellationToken ct);
    Seller Add(Seller seller);
    Seller Update(Seller seller);
    Seller Delete(Seller seller);
}
