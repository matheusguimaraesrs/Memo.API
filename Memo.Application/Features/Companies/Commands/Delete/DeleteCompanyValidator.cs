using FluentValidation;

namespace Memo.Application.Features.Companies.Commands.Delete;

internal sealed class DeleteCompanyValidator : AbstractValidator<DeleteCompanyCommand>
{
    public DeleteCompanyValidator()
    {
        RuleFor(company => company.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}