using MediatR;

namespace Invento.Application.Abstractions
{
    public interface IQuery<TResponse> : IRequest<TResponse>
    {
    }
}
