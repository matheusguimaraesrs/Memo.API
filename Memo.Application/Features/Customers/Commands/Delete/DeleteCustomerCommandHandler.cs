using Memo.Application.Common.CQRS;
using Memo.Domain.Common;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Customers.Commands.Delete;

internal sealed class DeleteCustomerCommandHandler(
    ICustomerRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCustomerCommand, Guid>
{
    public async Task<Guid> HandleAsync(DeleteCustomerCommand request, CancellationToken ct)
    {
        Customer? customer = await repository.GetByIdAsync(request.Id, ct);

        if (customer is null)
            throw new EntityNotFoundException<Customer>();

        repository.Delete(customer);
        await unitOfWork.SaveChangesAsync(ct);

        return customer.Id;
    }
}