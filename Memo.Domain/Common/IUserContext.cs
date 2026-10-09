namespace Memo.Domain.Common;

public interface IUserContext
{
    string? UserId { get; }
    string? UserName { get; }
}