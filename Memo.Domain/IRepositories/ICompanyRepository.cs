using Memo.Domain.Common;
using Memo.Domain.Entities;

namespace Memo.Domain.IRepositories;

public interface ICompanyRepository
{
    Task<PagedList<Company>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<Company?> GetByIdAsync(Guid id, CancellationToken ct);
    Company Add(Company company);
    Company Update(Company company);
    Company Delete(Company company);
}