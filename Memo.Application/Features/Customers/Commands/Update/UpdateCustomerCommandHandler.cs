using Memo.Application.Common.CQRS;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;
using Memo.Domain.ValueObjects;

namespace Memo.Application.Features.Customers.Commands.Update;

internal sealed class UpdateCustomerCommandHandler(
    ICustomerRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCustomerCommand, Guid>
{
    public async Task<Guid> HandleAsync(UpdateCustomerCommand request, CancellationToken ct)
    {
        Customer? customer = await repository.GetByIdAsync(request.Id, ct);

        if (customer is null)
            throw new EntityNotFoundException<Customer>();

        Address address = AddressMapper.ToDomain(request.Address);
        customer.Update(request.Name, address);
        
        repository.Update(customer);
        await unitOfWork.SaveChangesAsync(ct);

        return customer.Id;
    }
}