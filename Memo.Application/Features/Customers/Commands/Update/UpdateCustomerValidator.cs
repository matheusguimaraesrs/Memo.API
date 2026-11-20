using FluentValidation;

namespace Memo.Application.Features.Customers.Commands.Update;

internal sealed class UpdateCustomerValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerValidator()
    {
        RuleFor(customer => customer.Id)
            .NotEmpty().WithMessage("Id is required.");
        RuleFor(customer => customer.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be not exceed 100 characters.");
        RuleFor(customer => customer.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(255).WithMessage("Address must be not exceed 255 characters.");
    }
}