using Memo.Domain.Entities;

namespace Memo.Domain.IRepositories;

public interface ISellerRepository
{
    Task<Seller?> GetByIdAsync(Guid id);
    Task<IEnumerable<Seller>> GetAllAsync(); // mudar para PagedList futuramente
    Task<Seller> AddAsync(Seller seller);
    Task<Seller> UpdateAsync(Seller seller);
    Task<Seller> DeleteAsync(Seller seller);
}
