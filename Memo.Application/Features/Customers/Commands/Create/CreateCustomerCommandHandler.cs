using Memo.Application.Common.CQRS;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;
using Memo.Domain.ValueObjects;

namespace Memo.Application.Features.Customers.Commands.Create;

internal sealed class CreateCustomerCommandHandler(
    ICustomerRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateCustomerCommand request, CancellationToken ct)
    {
        Address address = AddressMapper.ToDomain(request.Address);
        Customer customer = new Customer(request.Name, address);

        repository.Add(customer);
        await unitOfWork.SaveChangesAsync(ct);

        return customer.Id;
    }
}