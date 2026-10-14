namespace Memo.Domain.Common;

public abstract class AuditableEntity : IAuditable
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public string? CreatedBy { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public string? UpdatedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public string? DeletedBy { get; protected set; }

    void IAuditable.WhoCreated(string? userName) => CreatedBy = userName;

    void IAuditable.UpdateAudit(string? userName)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userName;
    } 

    void IAuditable.SoftDelete(string? userName)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userName;
    }
}
