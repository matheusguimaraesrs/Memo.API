using FluentValidation;

namespace Memo.Application.Features.Sellers.Queries.GetAll;

internal sealed class GetAllSellersValidator : AbstractValidator<GetAllSellersQuery>
{
    public GetAllSellersValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThan(0).WithMessage("Page must be greater than zero.");
        RuleFor(query => query.PageSize)
            .InclusiveBetween(10, 100).WithMessage("PageSize must be between 10 and 100.");
    }
}