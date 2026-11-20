using FluentValidation;

namespace Memo.Application.Features.Customers.Commands.Create;

internal sealed class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(customer => customer.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be not exceed 100 characters.");
        
        RuleFor(customer => customer.Address)
            .NotEmpty().WithMessage("Address is required.");
        
        RuleFor(customer => customer.Address.PostalCode)
            .NotEmpty().WithMessage("Postal code is required.")
            .Must(pc => pc.Length == 8 && pc.All(char.IsDigit))
            .WithMessage("Postal code must contain 8 digits.");

        RuleFor(customer => customer.Address.Number)
            .NotEmpty().WithMessage("Address number is required.")
            .MaximumLength(20).WithMessage("Address number must not exceed 20 characters.");

        RuleFor(customer => customer.Address.Street)
            .NotEmpty().WithMessage("Street is required.")
            .MaximumLength(150).WithMessage("Street must not exceed 150 characters.");

        RuleFor(customer => customer.Address.Neighborhood)
            .NotEmpty().WithMessage("Neighborhood is required.")
            .MaximumLength(100).WithMessage("Neighborhood must not exceed 100 characters.");

        RuleFor(customer => customer.Address.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

        RuleFor(customer => customer.Address.State)
            .IsInEnum().WithMessage("State is invalid.");
    }
}