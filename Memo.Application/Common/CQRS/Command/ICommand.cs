namespace Memo.Application.Common.CQRS.Command;

public interface ICommand<out TResponse> : IRequest<TResponse> { }
