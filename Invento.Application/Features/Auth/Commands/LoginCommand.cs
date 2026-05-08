using MediatR;

namespace Invento.Application.Features.Auth.Commands
{
    public class LoginCommand : IRequest<string>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}