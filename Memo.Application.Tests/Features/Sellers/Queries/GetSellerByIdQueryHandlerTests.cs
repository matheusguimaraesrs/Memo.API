using FluentAssertions;
using Memo.Application.Features.Sellers.Queries.GetById;
using Memo.Domain.Entities;
using Memo.Domain.IRepositories;
using NSubstitute;

namespace Memo.Application.Tests.Features.Sellers.Queries;

public sealed class GetSellerByIdQueryHandlerTests
{
    private readonly ISellerRepository _repository = Substitute.For<ISellerRepository>();
    private readonly GetSellerByIdQueryHandler _handler;

    public GetSellerByIdQueryHandlerTests()
    {
        _handler = new GetSellerByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_WhenSellerExist()
    {
        List<Seller> sellers = new List<Seller>
        {
            new Seller("Seller One", "seller.1", "123"),
            new Seller("Seller Two", "seller.2", "123")
        };

        _repository.GetByIdAsync(sellers[0].Id).Returns(sellers[0]);

        var query = new GetSellerByIdQuery(sellers[0].Id);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(sellers[0].Id);
        result.Name.Should().Be("Seller One");
        result.Login.Should().Be("seller.1");

        await _repository.Received(1).GetByIdAsync(sellers[0].Id);
    }
}