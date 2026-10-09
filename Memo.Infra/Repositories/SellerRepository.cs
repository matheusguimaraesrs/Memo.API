using Memo.Domain.Entities;
using Memo.Domain.IRepositories;
using Memo.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Repositories;

internal sealed class SellerRepository(AppDbContext context) : ISellerRepository
{
    public async Task<Seller?> GetByIdAsync(Guid id)
    {
        return await context.Set<Seller>().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Seller>> GetAllAsync()
    {
        return await context.Set<Seller>().ToListAsync();
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