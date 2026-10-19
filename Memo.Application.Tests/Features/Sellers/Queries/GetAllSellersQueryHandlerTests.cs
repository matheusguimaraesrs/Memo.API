using FluentAssertions;
using Memo.Application.Features.Sellers.Queries.GetAll;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Sellers.Queries;

public sealed class GetAllSellersQueryHandlerTests
{
    private readonly ISellerRepository _repository = Substitute.For<ISellerRepository>();
    private readonly GetAllSellersQueryHandler _handler;

    public GetAllSellersQueryHandlerTests()
    {
        _handler = new GetAllSellersQueryHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_WhenSellersExist()
    {
        List<Seller> sellers = new List<Seller>
        {
            new Seller("Seller One", "seller.1", "123"),
            new Seller("Seller Two", "seller.2", "123")
        };

        _repository.GetAllAsync().Returns(sellers);
        
        var query = new GetAllSellersQuery();

        var result = await _handler.HandleAsync(query, CancellationToken.None);
        
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Select(x => x.Name).Should().Contain(new[] { "Seller One", "Seller Two" });

        await _repository.Received(1).GetAllAsync();
    }
}