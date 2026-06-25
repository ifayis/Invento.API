using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Auth.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Auth.Commands;

public class RegisterCommandHandler
    : ICommandHandler<
        RegisterCommand,
        ApiResponse<AuthResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ApiResponse<AuthResponseDto>>
        Handle(
            RegisterCommand request,
            CancellationToken cancellationToken)
    {
        var existingUser =
            await _context.Users
            .FirstOrDefaultAsync(
                x => x.Email == request.Email,
                cancellationToken);

        if (existingUser != null)
        {
            return ApiResponse<AuthResponseDto>
                .FailureResponse(
                    new List<string>
                    {
                        "Email already exists"
                    });
        }

        var tenant = new Tenant
        {
            CompanyName = request.CompanyName,
            Email = request.Email
        };

        await _context.Tenants.AddAsync(
            tenant,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash =
                PasswordHasher.Hash(
                    request.Password),

            Role = UserRole.Admin,
            TenantId = tenant.Id,
            MustChangePassword = false
        };

        await _context.Users.AddAsync(
            user,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        var accessToken =
            _jwtTokenGenerator
            .GenerateAccessToken(user);

        var refreshTokenValue =
            _jwtTokenGenerator
            .GenerateRefreshToken();

        var refreshToken =
            new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt =
                    DateTime.UtcNow.AddDays(7)
            };

        await _context.RefreshTokens
            .AddAsync(
                refreshToken,
                cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return ApiResponse<AuthResponseDto>
            .SuccessResponse(
                new AuthResponseDto
                {
                    AccessToken =
                        accessToken,

                    RefreshToken =
                        refreshTokenValue,

                    ExpiresAt =
                        DateTime.UtcNow.AddMinutes(15)
                },
                "Registration successful");
    }
}