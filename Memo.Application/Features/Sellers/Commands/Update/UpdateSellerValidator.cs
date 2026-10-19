using FluentValidation;

namespace Memo.Application.Features.Sellers.Commands.Update;

internal sealed class UpdateSellerValidator : AbstractValidator<UpdateSellerCommand>
{
    public UpdateSellerValidator()
    {
        RuleFor(seller => seller.Id)
            .NotEmpty().WithMessage("Id is required.");
        RuleFor(seller => seller.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be not exceed 100 characters.");
        RuleFor(seller => seller.Login)
            .NotEmpty().WithMessage("Login is required.")
            .MaximumLength(15).WithMessage("Login must be not exceed 15 characters.");
        RuleFor(seller => seller.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(20).WithMessage("Password must be not exceed 20 characters.");
    }
}