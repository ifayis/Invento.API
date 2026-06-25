using System.Security.Cryptography;
using System.Text;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Auth.Commands
{
    public class ForgotPasswordCommandHandler
        : ICommandHandler<
            ForgotPasswordCommand,
            ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;

        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(
            IApplicationDbContext context,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<ApiResponse<string>> Handle(
            ForgotPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var email =
                request.Email
                    .Trim()
                    .ToLower();

            var user =
                await _context.Users
                    .FirstOrDefaultAsync(
                        x =>
                            x.Email.ToLower() == email
                            && !x.IsDeleted,
                        cancellationToken);

            // Prevent email enumeration
            if (user is null)
            {
                return ApiResponse<string>
                    .SuccessResponse(
                        "If the email exists, a password reset link has been sent.");
            }

            var existingTokens =
                await _context.PasswordResetTokens
                    .Where(x =>
                        x.UserId == user.Id &&
                        !x.IsUsed)
                    .ToListAsync(cancellationToken);

            foreach (var token in existingTokens)
            {
                token.IsUsed = true;
                token.UsedAt = DateTime.UtcNow;
            }

            var rawToken =
                Convert.ToHexString(
                    RandomNumberGenerator.GetBytes(32));

            var tokenHash =
                Convert.ToHexString(
                    SHA256.HashData(
                        Encoding.UTF8.GetBytes(rawToken)));

            var passwordResetToken =
                new PasswordResetToken
                {
                    UserId = user.Id,

                    TokenHash = tokenHash,

                    ExpiresAt =
                        DateTime.UtcNow.AddMinutes(30)
                };

            await _context.PasswordResetTokens
                .AddAsync(
                    passwordResetToken,
                    cancellationToken);

            var body = $@"
                <h2>Password Reset</h2>

                <p>Hello {user.FullName},</p>

                <p>
                Use the following token to reset your password.
                </p>

                <h3>{rawToken}</h3>

                <p>
                This token expires in <b>30 minutes</b>.
                </p>

                <p>
                If you didn't request this reset,
                please ignore this email.
                </p>";

            await _emailService.SendEmailAsync(
                user.Email,
                "Invento Password Reset",
                body);

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<string>
                .SuccessResponse(
                    "If the email exists, a password reset link has been sent.");
        }
    }
}