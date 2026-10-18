namespace Memo.Domain.Exceptions;

public sealed class EntityNotFoundException<TEntity> : Exception
{
    public EntityNotFoundException() 
        : base($"{typeof(TEntity).Name} not found.") { }
}
