using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;
using Memo.Infra.Persistence;
using Memo.Infra.Services;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Repositories;

internal sealed class ProductRepository(AppDbContext context, IPaginationService pagination) : IProductRepository
{
    public async Task<PagedList<Product>> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Product> query = context.Products
            .AsNoTracking()
            .OrderBy(product => product.CreatedAt)
            .ThenBy(product => product.Id);

        return await pagination.PaginateAsync(query, page, pageSize, ct);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Products
            .FirstOrDefaultAsync(product => product.Id == id, ct);
    }

    public Product Add(Product product)
    {
        context.Products.Add(product);
        return product;
    }

    public Product Update(Product product)
    {
        context.Products.Update(product);
        return product;
    }

    public Product Delete(Product product)
    {
        context.Products.Remove(product);
        return product;
    }
}