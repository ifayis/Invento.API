using MediatR;
using Invento.Application.Features.Auth.Models;

public class RefreshTokenCommand : IRequest<AuthResponse>
{
    public string RefreshToken { get; set; } = string.Empty;
}