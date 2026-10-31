using Memo.Domain.Common;
using Memo.Domain.Entities;

namespace Memo.Domain.IRepositories;

public interface ICompanyRepository
{
    Task<PagedList<Company>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Company?> GetByIdAsync(Guid id);
    Company Add(Company company);
    Company Update(Company company);
    Company Delete(Company company);
}