using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Auth.Commands
{
    public class LogoutCommandHandler
        : ICommandHandler<
            LogoutCommand,
            ApiResponse<string>>
    {
        private readonly IApplicationDbContext
            _context;

        public LogoutCommandHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> Handle(
            LogoutCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(
                request.RefreshToken))
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new List<string>
                        {
                            "Refresh token is required."
                        },
                        "Logout failed");
            }

            var refreshTokenHash =
                RefreshTokenHasher.Hash(
                    request.RefreshToken);

            var token =
                await _context.RefreshTokens
                    .FirstOrDefaultAsync(
                        x =>
                            x.Token ==
                                refreshTokenHash,
                        cancellationToken);

            if (token is null ||
                token.IsRevoked)
            {
                return ApiResponse<string>
                    .SuccessResponse(
                        "Logged out successfully.");
            }

            token.IsRevoked = true;

            token.RevokedAt =
                DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<string>
                .SuccessResponse(
                    "Logged out successfully.");
        }
    }
}