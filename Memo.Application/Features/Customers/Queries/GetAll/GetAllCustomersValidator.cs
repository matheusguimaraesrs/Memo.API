using FluentValidation;

namespace Memo.Application.Features.Customers.Queries.GetAll;

internal sealed class GetAllCustomersValidator : AbstractValidator<GetAllCustomersQuery>
{
    public GetAllCustomersValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThan(0).WithMessage("Page must be greater than zero.");
        
        RuleFor(query => query.PageSize)
            .InclusiveBetween(10, 100).WithMessage("Page size must be between 10 and 100.");
    }
}