namespace Memo.Domain.Common;

public interface IAuditable
{
    void WhoCreated(string? userName);
    void UpdateAudit(string? userName);
    void SoftDelete(string? userName);
}