using Memo.Domain.Common;
using Memo.Domain.Entities;

namespace Memo.Domain.IRepositories;

public interface IProductRepository
{
    Task<PagedList<Product>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
    Product Add(Product product);
    Product Update(Product product);
    Product Delete(Product product);
}