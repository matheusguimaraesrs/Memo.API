using FluentValidation;

namespace Memo.Application.Features.Companies.Commands.Update;

internal sealed class UpdateCompanyValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyValidator()
    {
        RuleFor(company => company.Id)
            .NotEmpty().WithMessage("Id is required.");
        RuleFor(company => company.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be not exceed 100 characters.");
        RuleFor(company => company.Commission)
            .InclusiveBetween(0m, 100m).WithMessage("Commission must be between 0% and 100%");
    }
}