using FluentValidation;

namespace Memo.Application.Features.Customers.Commands.Delete;

internal sealed class DeleteCustomerValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerValidator()
    {
        RuleFor(customer => customer.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}