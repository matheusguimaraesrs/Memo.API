using Memo.Domain.Common;
using Memo.Domain.Entities;

namespace Memo.Domain.IRepositories;

public interface IRepresentedCompanyRepository
{
    Task<PagedList<RepresentedCompany>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<RepresentedCompany?> GetByIdAsync(Guid id);
    RepresentedCompany Add(RepresentedCompany representedCompany);
    RepresentedCompany Update(RepresentedCompany representedCompany);
    RepresentedCompany Delete(RepresentedCompany representedCompany);
}