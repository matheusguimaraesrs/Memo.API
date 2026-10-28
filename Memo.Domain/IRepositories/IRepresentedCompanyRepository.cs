using Memo.Domain.Common;
using Memo.Domain.Entities;

namespace Memo.Domain.IRepositories;

public interface IRepresentedCompanyRepository
{
    Task<PagedList<RepresentedCompany>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<RepresentedCompany?> GetByIdAsync(Guid id);
    Task<RepresentedCompany> AddAsync(RepresentedCompany representedCompany);
    Task<RepresentedCompany> UpdateAsync(RepresentedCompany representedCompany);
    Task<RepresentedCompany> DeleteAsync(RepresentedCompany representedCompany);
}