using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Common.Security;
using Invento.Application.Features.Users.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Users.Queries
{
    public class GetUserByIdQueryHandler
        : IQueryHandler<
            GetUserByIdQuery,
            ApiResponse<UserDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetUserByIdQueryHandler(
            IDbConnectionFactory connectionFactory,
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _connectionFactory = connectionFactory;
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<UserDto>> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            const string sql = @"
            SELECT
                Id,
                TenantId,
                FullName,
                Email,
                Role,
                IsActive,
                MustChangePassword,
                CreatedAt,
                CreatedBy,
                CreatedByUserId
            FROM Users
            WHERE
                Id = @Id
                AND IsDeleted = 0;
            ";

            var targetUser =
                await connection.QuerySingleOrDefaultAsync<UserDto>(
                    sql,
                    new
                    {
                        request.Id
                    });

            if (targetUser is null)
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "User not found."
                        });
            }

            if (!Guid.TryParse(
                _currentUser.UserId,
                out var currentUserId))
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "Unauthorized."
                        });
            }

            var currentUser =
                await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == currentUserId &&
                            !x.IsDeleted,
                        cancellationToken);

            if (currentUser is null)
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "Unauthorized."
                        });
            }

            var authorized =
                UserAuthorizationService.CanViewUser(
                    currentUser.Role,
                    currentUser.TenantId,
                    currentUser.Id,
                    targetUser.Role,
                    targetUser.TenantId,
                    targetUser.Id);

            if (!authorized)
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "You are not authorized to view this user."
                        });
            }

            return ApiResponse<UserDto>
                .SuccessResponse(
                    targetUser);
        }
    }
}