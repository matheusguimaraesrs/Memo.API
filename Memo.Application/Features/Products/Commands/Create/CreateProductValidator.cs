using FluentValidation;

namespace Memo.Application.Features.Products.Commands.Create;

internal sealed class CreateProductValidator: AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(product => product.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        RuleFor(product => product.Description)
            .MaximumLength(255).WithMessage("Description must not exceed 255 characters.");
        RuleFor(product => product.UnitPrice)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(product => product.CompanyId)
            .NotEmpty().WithMessage("Company is required.");
    }
}