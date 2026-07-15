using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Common.Extensions;
using Invento.Application.Common.Security;
using Invento.Application.Features.Users.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Users.Commands
{
    public class CreateUserCommandHandler
        : ICommandHandler<
            CreateUserCommand,
            ApiResponse<UserDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IEmailService _emailService;
        private readonly ICurrentTenantService _currentTenant;
        private readonly ICacheVersionService _cacheVersionService;

        public CreateUserCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IEmailService emailService,
            ICurrentTenantService currentTenant,
            ICacheVersionService cacheVersionService)
        {
            _context = context;
            _currentUser = currentUser;
            _emailService = emailService;
            _currentTenant = currentTenant;
            _cacheVersionService = cacheVersionService;
        }

        public async Task<ApiResponse<UserDto>> Handle(
            CreateUserCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentTenant.TenantId;

            if (!Guid.TryParse(
                _currentUser.UserId,
                out var currentUserId))
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "Unauthorized user."
                        }
                    );
            }

            var creator =
                await _context.Users
                    .Include(x => x.Tenant)
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == currentUserId &&
                            !x.IsDeleted,
                        cancellationToken
                    );

            if (creator is null)
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "User not found."
                        }
                    );
            }

            if (!creator.IsActive)
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "Your account has been deactivated."
                        }
                    );
            }

            if (!UserAuthorizationService.CanCreateUser(
                creator.Role,
                request.Role))
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                          "You are not authorized to create this type of user."
                        }
                    );
            }

            var normalizedEmail =
                request.Email
                    .Trim()
                    .ToLowerInvariant();

            var emailExists =
                await _context.Users
                    .AnyAsync(
                        x =>
                            x.Email.ToLower() == normalizedEmail,
                        cancellationToken
                    );

            if (emailExists)
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "Email already exists."
                        }
                    );
            }

            var user =
                new User
                {
                    TenantId = creator.TenantId,

                    FullName =
                        request.FullName.Trim(),

                    Email =
                        normalizedEmail,

                    PasswordHash =
                        PasswordHasher.Hash(
                            request.Password),

                    Role = request.Role,

                    IsActive = true,

                    MustChangePassword = true,

                    CreatedByUserId = creator.Id
                };

            await _context.Users.AddAsync(
                user,
                cancellationToken
            );

            await _context.SaveChangesAsync(
                cancellationToken);

            await _cacheVersionService.InvalidateAsync(
            tenantId,
            CacheGroups.Users);

            await _emailService.SendEmailAsync(
                user.Email,
                "Your Invento Account",
                EmailTemplates.TemporaryPassword(
                    user.FullName,
                    creator.Tenant.CompanyName,
                    user.Email,
                    request.Password,
                    user.Role.ToString()
                )
            );

            return ApiResponse<UserDto>
                .SuccessResponse(
                    new UserDto
                    {
                        Id = user.Id,

                        FullName = user.FullName,

                        Email = user.Email,

                        Role = user.Role,

                        IsActive = user.IsActive,

                        MustChangePassword =
                            user.MustChangePassword,

                        CreatedAt = user.CreatedAt
                    },
                    "User created successfully."
                );
        }
    }
}