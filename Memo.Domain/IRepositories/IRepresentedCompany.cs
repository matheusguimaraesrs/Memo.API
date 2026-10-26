using Memo.Domain.Entities;

namespace Memo.Domain.IRepositories;

public interface IRepresentedCompany
{
    Task<RepresentedCompany?> GetByIdAsync();
    Task<IEnumerable<RepresentedCompany>> GetAllAsync();
    Task<Seller> AddAsync(RepresentedCompany representedCompany);
    Task<Seller> UpdateAsync(RepresentedCompany representedCompany);
    Task<Seller> DeleteAsync(RepresentedCompany representedCompany);
}