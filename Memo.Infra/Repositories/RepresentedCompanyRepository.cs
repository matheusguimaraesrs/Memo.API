using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;
using Memo.Infra.Persistence;
using Memo.Infra.Services;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Repositories;

internal sealed class RepresentedCompanyRepository(AppDbContext context, IPaginationService pagination) : IRepresentedCompanyRepository
{
    public async Task<PagedList<RepresentedCompany>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        IQueryable<RepresentedCompany> query = context.Set<RepresentedCompany>()
            .AsNoTracking()
            .OrderBy(company => company.CreatedAt)
            .ThenBy(company => company.Id);

        return await pagination.PaginateAsync(query, page, pageSize, ct);
    }

    public async Task<RepresentedCompany?> GetByIdAsync(Guid id)
    {
        return await context.Set<RepresentedCompany>().FirstOrDefaultAsync(c => c.Id == id);
    }

    public RepresentedCompany Add(RepresentedCompany representedCompany)
    {
        context.RepresentedCompanies.Add(representedCompany);
        return representedCompany;
    }

    public RepresentedCompany Update(RepresentedCompany representedCompany)
    {
        context.RepresentedCompanies.Update(representedCompany);
        return representedCompany;
    }

    public RepresentedCompany Delete(RepresentedCompany representedCompany)
    {
        context.Set<RepresentedCompany>().Remove(representedCompany);
        return representedCompany;
    }
}