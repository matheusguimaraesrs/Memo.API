using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;
using Memo.Infra.Persistence;
using Memo.Infra.Services;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Repositories;

internal sealed class CompanyRepository(AppDbContext context, IPaginationService pagination) : ICompanyRepository
{
    public async Task<PagedList<Company>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        IQueryable<Company> query = context.Set<Company>()
            .AsNoTracking()
            .OrderBy(company => company.CreatedAt)
            .ThenBy(company => company.Id);

        return await pagination.PaginateAsync(query, page, pageSize, ct);
    }

    public async Task<Company?> GetByIdAsync(Guid id)
    {
        return await context.Set<Company>().FirstOrDefaultAsync(c => c.Id == id);
    }

    public Company Add(Company company)
    {
        context.Companies.Add(company);
        return company;
    }

    public Company Update(Company company)
    {
        context.Companies.Update(company);
        return company;
    }

    public Company Delete(Company company)
    {
        context.Set<Company>().Remove(company);
        return company;
    }
}