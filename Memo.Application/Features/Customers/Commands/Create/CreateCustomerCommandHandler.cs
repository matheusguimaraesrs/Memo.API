using Memo.Application.Common.CQRS;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Customers.Commands.Create;

internal sealed class CreateCustomerCommandHandler(
    ICustomerRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateCustomerCommand request, CancellationToken ct)
    {
        Customer customer = new Customer(request.Name, request.Address);

        repository.Add(customer);
        await unitOfWork.SaveChangesAsync(ct);

        return customer.Id;
    }
}