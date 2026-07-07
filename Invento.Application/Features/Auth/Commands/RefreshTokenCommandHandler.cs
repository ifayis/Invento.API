using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Auth.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Invento.Application.Features.Auth.Commands;

public class RefreshTokenCommandHandler
    : ICommandHandler<RefreshTokenCommand, ApiResponse<AuthResponseDto>>
{
    private readonly IApplicationDbContext _context;

    private readonly IJwtTokenGenerator
        _jwtTokenGenerator;

    private readonly JwtSettings
        _jwtSettings;

    public RefreshTokenCommandHandler(
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
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
    {
        var refreshTokenHash =
            RefreshTokenHasher.Hash(
            request.RefreshToken);

        var storedToken =
            await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x =>
                    x.Token == refreshTokenHash
                    && !x.IsRevoked,
                cancellationToken);

        if (storedToken is null)
        {
            return ApiResponse<AuthResponseDto>
                .FailureResponse(
                    new List<string>
                    {
                        "Invalid refresh token"
                    },
                    "Unauthorized"
                );
        }

        if (storedToken.ExpiresAt < DateTime.UtcNow)
        {
            return ApiResponse<AuthResponseDto>
                .FailureResponse(
                    new List<string>
                    {
                        "Refresh token expired"
                    },
                    "Unauthorized"
                );
        }

        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;

        var newRefreshToken =
            _jwtTokenGenerator.GenerateRefreshToken();

        var newRefreshTokenHash =
            RefreshTokenHasher.Hash(
                newRefreshToken);

        var refreshTokenEntity =
            new RefreshToken
            {
                UserId = storedToken.UserId,

                Token = newRefreshTokenHash,

                ExpiresAt =
                    DateTime.UtcNow.AddDays(
                        _jwtSettings
                            .RefreshTokenExpirationDays),
                                IsRevoked = false
            };

        await _context.RefreshTokens
            .AddAsync(
                refreshTokenEntity,
                cancellationToken);

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(storedToken.User);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<AuthResponseDto>
            .SuccessResponse(
                new AuthResponseDto
                {
                    AccessToken =
                        accessToken,

                    RefreshToken =
                        newRefreshToken,

                    ExpiresAt =
                        DateTime.UtcNow.AddMinutes(
                            _jwtSettings
                                .AccessTokenExpirationMinutes),

                    MustChangePassword =
                        storedToken.User
                            .MustChangePassword
                },
                "Token refreshed successfully"
            );
    }
}