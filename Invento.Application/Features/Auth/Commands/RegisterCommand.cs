using MediatR;

namespace Invento.Application.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<string>
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string CompanyCode { get; set; } = string.Empty;

        public string? LogoUrl { get; set; }

        public string? BusinessPurpose { get; set; }
    }
}