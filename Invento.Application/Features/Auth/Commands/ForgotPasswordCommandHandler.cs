using System.Security.Cryptography;
using System.Text;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Invento.Application.Features.Auth.Commands
{
    public class ForgotPasswordCommandHandler
        : ICommandHandler<
            ForgotPasswordCommand,
            ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly FrontendSettings _frontendSettings;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(
            IApplicationDbContext context,
            IEmailService emailService,
            IOptions<FrontendSettings> frontendOptions)
        {
            _context = context;
            _emailService = emailService;
            _frontendSettings = frontendOptions.Value;
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

            var frontendUrl =
                _frontendSettings.Url.TrimEnd('/');

            var resetLink =
                $"{frontendUrl}/reset-password?token=" +
                Uri.EscapeDataString(rawToken);

            var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset=""UTF-8"" />
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                <title>Invento Password Reset</title>
            </head>

            <body style=""
                margin: 0;
                padding: 0;
                background-color: #f8fafc;
                font-family: Arial, Helvetica, sans-serif;
            "">

                <div style=""
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 40px 20px;
                "">

                    <div style=""
                        background-color: #ffffff;
                        border-radius: 16px;
                        padding: 40px;
                        border: 1px solid #e2e8f0;
                    "">

                        <h2 style=""
                            margin: 0 0 20px;
                            color: #0f172a;
                        "">
                            Reset your INVENTO password
                        </h2>

                        <p style=""
                            color: #475569;
                            line-height: 1.6;
                        "">
                            Hello {System.Net.WebUtility.HtmlEncode(user.FullName)},
                        </p>

                        <p style=""
                            color: #475569;
                            line-height: 1.6;
                        "">
                            We received a request to reset the password
                            for your INVENTO account.
                        </p>

                        <div style=""
                            margin: 30px 0;
                            text-align: center;
                        "">

                            <a
                                href=""{resetLink}""
                                style=""
                                    display: inline-block;
                                    padding: 14px 24px;
                                    background-color: #0f172a;
                                    color: #ffffff;
                                    text-decoration: none;
                                    border-radius: 10px;
                                    font-weight: 600;
                                ""
                            >
                                Reset Password
                            </a>

                        </div>

                        <p style=""
                            color: #64748b;
                            font-size: 14px;
                            line-height: 1.6;
                        "">
                            This password reset link expires in
                            <strong>30 minutes</strong>.
                        </p>

                        <p style=""
                            color: #64748b;
                            font-size: 14px;
                            line-height: 1.6;
                        "">
                            If you did not request a password reset,
                            you can safely ignore this email.
                        </p>

                        <hr style=""
                            border: 0;
                            border-top: 1px solid #e2e8f0;
                            margin: 30px 0;
                        "" />

                        <p style=""
                            color: #94a3b8;
                            font-size: 12px;
                            line-height: 1.5;
                        "">
                            If the button does not work, copy and paste
                            the following URL into your browser:
                        </p>

                        <p style=""
                            color: #64748b;
                            font-size: 12px;
                            word-break: break-all;
                        "">
                            {System.Net.WebUtility.HtmlEncode(resetLink)}
                        </p>

                    </div>

                    <p style=""
                        text-align: center;
                        color: #94a3b8;
                        font-size: 12px;
                        margin-top: 20px;
                    "">
                        © {DateTime.UtcNow.Year} INVENTO
                    </p>

                </div>

            </body>
            </html>";

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