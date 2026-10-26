using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;
using Memo.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Repositories;

internal sealed class SellerRepository(AppDbContext context) : ISellerRepository
{
    public async Task<PagedList<Seller>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        IQueryable<Seller> query = context.Set<Seller>()
            .AsNoTracking()
            .OrderBy(seller => seller.CreatedAt)
            .ThenBy(seller => seller.Id);
        
        int totalItems = await query.CountAsync(ct);
        List<Seller> sellers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedList<Seller>(sellers, page, pageSize, totalItems);
    }

    public async Task<Seller?> GetByIdAsync(Guid id)
    {
        return await context.Set<Seller>().FirstOrDefaultAsync(c => c.Id == id);
    }
 
    public async Task<Seller> AddAsync(Seller seller)
    {
        await context.Sellers.AddAsync(seller);
        await context.SaveChangesAsync();
        return seller;
    }

    public async Task<Seller> UpdateAsync(Seller seller)
    {
        context.Sellers.Update(seller);
        await context.SaveChangesAsync();
        return seller;
    }

    public async Task<Seller> DeleteAsync(Seller seller)
    {
        context.Set<Seller>().Remove(seller);
        await context.SaveChangesAsync();
        return seller;
    }
}