using FluentValidation;

namespace Memo.Application.Features.Sellers.Commands.Delete;

internal sealed class DeleteSellerValidator : AbstractValidator<DeleteSellerCommand>
{
    public DeleteSellerValidator()
    {
        RuleFor(seller => seller.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}