using System.Security.Claims;
using FluentAssertions;
using Memo.Infra.Services;
using Microsoft.AspNetCore.Http;

namespace Memo.Infra.Tests.DbContext;

public sealed class UserContextTests
{
    [Fact]
    public void Should_Return_UserId_And_UserName()
    {
        var httpContext = new DefaultHttpContext();

        httpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, "userId-123"),
                    new Claim(ClaimTypes.Name, "User Name")
                ],
                authenticationType: "Test"));
        var accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var userContext = new UserContext(accessor);

        userContext.UserId.Should().Be("userId-123");
        userContext.UserName.Should().Be("User Name");
    }
}