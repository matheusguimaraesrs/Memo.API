using Memo.Application.Common.CQRS;
using Memo.Application.Features.Products.Responses;
using Memo.Domain.Entities;
using Memo.Domain.Exceptions;
using Memo.Domain.IRepositories;

namespace Memo.Application.Features.Products.Queries.GetById;

internal sealed class GetProductByIdQueryHandler(IProductRepository repository)
    : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<ProductResponse> HandleAsync(GetProductByIdQuery request, CancellationToken ct)
    {
        Product? product = await repository.GetByIdAsync(request.Id, ct);

        if (product is null)
            throw new EntityNotFoundException<Product>();

        return new ProductResponse(
            product.Name, 
            product.Description, 
            product.UnitPrice, 
            product.CompanyId);
    }
}