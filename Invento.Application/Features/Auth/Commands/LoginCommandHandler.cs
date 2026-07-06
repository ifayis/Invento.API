using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Auth.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Auth.Commands
{
    public class LoginCommandHandler
        : ICommandHandler<LoginCommand, ApiResponse<AuthResponseDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginCommandHandler(
            IApplicationDbContext context,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(
                LoginCommand request,
                CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Email.Trim().ToLower();

            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Email.ToLower() == normalizedEmail
                    && !x.IsDeleted,
                    cancellationToken);

            if (user is null)
            {
                return ApiResponse<AuthResponseDto>
                    .FailureResponse(
                        new List<string>
                        {
                        "Invalid credentials"
                        },
                        "Login failed"
                    );
            }

            if (!user.IsActive)
            {
                return ApiResponse<AuthResponseDto>
                    .FailureResponse(
                        new()
                        {
                "Your account has been deactivated."
                        },
                        "Login failed");
            }

            var passwordValid =
                PasswordHasher.Verify(
                    request.Password,
                    user.PasswordHash
                );

            if (!passwordValid)
            {
                return ApiResponse<AuthResponseDto>
                    .FailureResponse(
                        new List<string>
                        {
                        "Invalid credentials"
                        },
                        "Login failed"
                    );
            }

            var oldTokens = await _context.RefreshTokens
                .Where(x =>
                    x.UserId == user.Id
                    && !x.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var token in oldTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);

            var refreshTokenValue =
                _jwtTokenGenerator.GenerateRefreshToken();

            var refreshTokenHash =
                RefreshTokenHasher.Hash(
                    refreshTokenValue);

            var refreshToken =
                new RefreshToken
                {
                    UserId = user.Id,

                    Token = refreshTokenHash,

                    ExpiresAt =
                        DateTime.UtcNow.AddDays(7),

                    IsRevoked = false
                };
            await _context.RefreshTokens
                .AddAsync(
                    refreshToken,
                    cancellationToken
                );

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<AuthResponseDto>
                .SuccessResponse(
                    new AuthResponseDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshTokenValue,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                        MustChangePassword = user.MustChangePassword
                    },
                    "Login successful");
        }
    }
}