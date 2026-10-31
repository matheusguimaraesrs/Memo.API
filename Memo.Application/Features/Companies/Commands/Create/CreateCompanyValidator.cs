using FluentValidation;

namespace Memo.Application.Features.Companies.Commands.Create;

internal sealed class CreateCompanyValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyValidator()
    {
        RuleFor(company => company.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be not exceed 100 characters");
        RuleFor(company => company.Commission)
            .InclusiveBetween(0m, 100m).WithMessage("Commission must be between 0 and 100.");
    }
}