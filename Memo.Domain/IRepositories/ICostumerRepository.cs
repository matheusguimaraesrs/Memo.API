using Memo.Domain.Common;
using Memo.Domain.Entities;

namespace Memo.Domain.IRepositories;

public interface ICostumerRepository
{
    Task<PagedList<Customer>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct);
    Customer Add(Customer customer);
    Customer Update(Customer customer);
    Customer Delete(Customer customer);
}