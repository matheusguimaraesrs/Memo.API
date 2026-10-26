using Memo.Domain.Common;
using Memo.Domain.Entities;

namespace Memo.Domain.IRepositories;

public interface ISellerRepository
{
    Task<PagedList<Seller>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Seller?> GetByIdAsync(Guid id);
    Task<Seller> AddAsync(Seller seller);
    Task<Seller> UpdateAsync(Seller seller);
    Task<Seller> DeleteAsync(Seller seller);
}
