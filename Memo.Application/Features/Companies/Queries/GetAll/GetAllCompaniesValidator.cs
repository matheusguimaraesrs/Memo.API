using FluentValidation;

namespace Memo.Application.Features.Companies.Queries.GetAll;

internal sealed class GetAllCompaniesValidator : AbstractValidator<GetAllCompaniesQuery>
{
    public GetAllCompaniesValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThan(0).WithMessage("Page must be greater than zero.");
        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 10 and 100.");
    }
}