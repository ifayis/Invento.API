using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Auth.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Invento.Application.Features.Auth.Commands
{
    public class LoginCommandHandler
        : ICommandHandler<LoginCommand, ApiResponse<AuthResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        private readonly IJwtTokenGenerator
            _jwtTokenGenerator;

        private readonly JwtSettings
            _jwtSettings;

        public LoginCommandHandler(
            IApplicationDbContext context,
            IJwtTokenGenerator jwtTokenGenerator,
            IOptions<JwtSettings> jwtOptions)
        {
            _context = context;

            _jwtTokenGenerator =
                jwtTokenGenerator;

            _jwtSettings =
                jwtOptions.Value;
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
                        DateTime.UtcNow.AddDays(
                            _jwtSettings
                                .RefreshTokenExpirationDays),
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
                        ExpiresAt =
                            DateTime.UtcNow.AddMinutes(
                                _jwtSettings
                                    .AccessTokenExpirationMinutes),
                        MustChangePassword = user.MustChangePassword
                    },
                    "Login successful");
        }
    }
}