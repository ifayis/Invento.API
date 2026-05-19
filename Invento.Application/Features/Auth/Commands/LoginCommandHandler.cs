using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Auth.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Invento.Application.Features.Auth.Commands
{
    public class LoginCommandHandler
        : ICommandHandler<
            LoginCommand,
            ApiResponse<AuthResponseDto>>
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
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Email == request.Email,
                    cancellationToken);

            if (user is null)
            {
                return ApiResponse<AuthResponseDto>
                    .FailureResponse(
                        new List<string>
                        {
                        "Invalid credentials"
                        });
            }

            var passwordValid =
                PasswordHasher.Verify(
                    request.Password,
                    user.PasswordHash);

            if (!passwordValid)
            {
                return ApiResponse<AuthResponseDto>
                    .FailureResponse(
                        new List<string>
                        {
                        "Invalid credentials"
                        });
            }

            var accessToken =
                _jwtTokenGenerator.GenerateAccessToken(user);

            var refreshTokenValue =
                _jwtTokenGenerator.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _context.RefreshTokens.AddAsync(
                refreshToken,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<AuthResponseDto>
                .SuccessResponse(
                    new AuthResponseDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshTokenValue,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(15)
                    },
                    "Login successful");
        }
    }
}