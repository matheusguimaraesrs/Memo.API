using Memo.Domain.Common;
using Memo.Domain.Entities;

namespace Memo.Domain.IRepositories;

public interface IRepresentedCompany
{
    Task<PagedList<RepresentedCompany>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<RepresentedCompany?> GetByIdAsync(Guid id);
    Task<Seller> AddAsync(RepresentedCompany representedCompany);
    Task<Seller> UpdateAsync(RepresentedCompany representedCompany);
    Task<Seller> DeleteAsync(RepresentedCompany representedCompany);
}