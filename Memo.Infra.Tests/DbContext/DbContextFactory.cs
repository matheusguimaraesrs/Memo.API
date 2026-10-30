using Memo.Domain.Common;
using Memo.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memo.Infra.Tests.DbContext;

internal static class DbContextFactory
{
    public static AppDbContext Create(IUserContext? userContext = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options, userContext ?? new FakeUserContext());
    }
    
    private sealed class FakeUserContext : IUserContext
    {
        public string? Id => null;
        public string? Name => "test-user";
    }
}