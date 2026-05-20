using MediatR;

namespace Invento.Application.Abstractions
{
    public interface ICommand<TResponse> : IRequest<TResponse>
    {
    }
}
