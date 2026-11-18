using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;
using Memo.Infra.Persistence;
using Memo.Infra.Services;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Repositories;

internal sealed class SellerRepository(AppDbContext context, IPaginationService pagination) : ISellerRepository
{
    public async Task<PagedList<Seller>> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Seller> query = context.Sellers
            .AsNoTracking()
            .OrderBy(seller => seller.CreatedAt)
            .ThenBy(seller => seller.Id);

        return await pagination.PaginateAsync(query, page, pageSize, ct);
    }

    public async Task<Seller?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Sellers
            .FirstOrDefaultAsync(seller => seller.Id == id, ct);
    }
 
    public Seller Add(Seller seller)
    {
        context.Sellers.Add(seller);
        return seller;
    }

    public Seller Update(Seller seller)
    {
        context.Sellers.Update(seller);
        return seller;
    }

    public Seller Delete(Seller seller)
    {
        context.Sellers.Remove(seller);
        return seller;
    }
}