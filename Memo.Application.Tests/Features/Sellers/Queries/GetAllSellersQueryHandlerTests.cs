using FluentAssertions;
using Memo.Application.Features.Sellers.Queries.GetAll;
using Memo.Domain.Common;
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

        _repository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>())
            .Returns(new PagedList<Seller>(sellers, 1, 10, 12));
        
        var query = new GetAllSellersQuery(1, 10);

        var result = await _handler.HandleAsync(query, CancellationToken.None);
        
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Select(x => x.Name).Should().Contain(new[] { "Seller One", "Seller Two" });
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalItems.Should().Be(12);
        result.TotalPages.Should().Be(2);

        await _repository.Received(1).GetPagedAsync(1, 10, Arg.Any<CancellationToken>());
    }
}