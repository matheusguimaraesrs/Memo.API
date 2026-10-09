using Memo.Domain.Entities;
using Memo.Domain.Interfaces;
using Memo.Infra.Persistence;

namespace Memo.Infra.Repositories;

internal sealed class SellerRepository : ISellerRepository
{
    private readonly AppDbContext _context;

    public SellerRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public Task<Seller?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Seller>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Seller> AddAsync(Seller seller)
    {
        await _context.Sellers.AddAsync(seller);
        await _context.SaveChangesAsync();
        return seller;
    }

    public Task<Seller> UpdateAsync(Seller seller)
    {
        throw new NotImplementedException();
    }

    public Task<Seller> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}