using FluentValidation;

namespace Memo.Application.Features.Products.Commands.Update;

internal sealed class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(product => product.Id)
            .NotEmpty().WithMessage("Id is required.");
        RuleFor(product => product.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be not exceed 100 characters.");
        RuleFor(product => product.Description)
            .MaximumLength(255).WithMessage("Description must not exceed 255 characters.");
        RuleFor(product => product.UnitPrice)
            .NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(product => product.CompanyId)
            .NotEmpty().WithMessage("Company is required.");
    }
}