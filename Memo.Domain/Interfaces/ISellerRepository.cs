using Memo.Domain.Entities;

namespace Memo.Domain.Interfaces;

public interface ISellerRepository
{
    Task<Seller?> GetByIdAsync(Guid id);
    Task<List<Seller>> GetAllAsync(); // mudar para PagedList futuramente
    Task<Seller> AddAsync(Seller seller);
    Task<Seller> UpdateAsync(Seller seller);
    Task<Seller> DeleteAsync(Guid id);
}
