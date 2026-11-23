using FluentValidation;

namespace Memo.Application.Features.Products.Commands.Delete;

internal sealed class DeleteProductValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductValidator()
    {
        RuleFor(product => product.id)
            .NotEmpty().WithMessage("Id is required.");
    }
}