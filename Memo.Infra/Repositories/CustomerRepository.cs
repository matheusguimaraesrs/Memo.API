using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;
using Memo.Infra.Persistence;
using Memo.Infra.Services;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Repositories;

internal sealed class CustomerRepository(AppDbContext context, IPaginationService pagination) : ICustomerRepository
{
    public async Task<PagedList<Customer>> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Customer> query = context.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.CreatedAt)
            .ThenBy(customer => customer.Id);

        return await pagination.PaginateAsync(query, page, pageSize, ct);
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Customers
            .FirstOrDefaultAsync(customer => customer.Id == id, ct);
    }

    public Customer Add(Customer customer)
    {
        context.Customers.Add(customer);
        return customer;
    }

    public Customer Update(Customer customer)
    {
        context.Customers.Update(customer);
        return customer;
    }

    public Customer Delete(Customer customer)
    {
        context.Customers.Remove(customer);
        return customer;
    }
}