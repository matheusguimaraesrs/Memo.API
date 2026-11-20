using Memo.Application.Common.CQRS;
using Memo.Application.Features.Customers.Response;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Customers.Queries.GetById;

internal sealed class GetCustomerByIdQueryHandler(ICustomerRepository repository)
    : IRequestHandler<GetCustomerByIdQuery, CustomerResponse>
{
    public async Task<CustomerResponse> HandleAsync(GetCustomerByIdQuery request, CancellationToken ct)
    {
        Customer? customer = await repository.GetByIdAsync(request.Id, ct);

        if (customer is null)
            throw new EntityNotFoundException<Customer>();

        return new CustomerResponse(customer.Id, customer.Name, customer.Address);
    }
}