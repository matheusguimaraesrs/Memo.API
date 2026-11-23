using Memo.Application.Common.CQRS;
using Memo.Application.Features.Customers.Responses;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Customers.Queries.GetAll;

internal sealed class GetAllCustomersQueryHandler(ICustomerRepository repository)
    : IRequestHandler<GetAllCustomersQuery, PagedList<CustomerResponse>>
{
    public async Task<PagedList<CustomerResponse>> HandleAsync(GetAllCustomersQuery request, CancellationToken ct)
    {
        PagedList<Customer> pagedList = await repository.GetPagedAsync(
            request.Page,
            request.PageSize,
            ct);

        IReadOnlyList<CustomerResponse> customers = pagedList.Items
            .Select(CustomerResponseMapper.ToResponse)
            .ToList();

        return new PagedList<CustomerResponse>(
            customers,
            pagedList.Page,
            pagedList.PageSize,
            pagedList.TotalItems);
    }
}