using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Users.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Users.Queries
{
    public class GetMyProfileQueryHandler
        : IQueryHandler<
            GetMyProfileQuery,
            ApiResponse<UserDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentUserService _currentUser;

        public GetMyProfileQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentUserService currentUser)
        {
            _connectionFactory = connectionFactory;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<UserDto>> Handle(
            GetMyProfileQuery request,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(
                _currentUser.UserId,
                out var userId))
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "Unauthorized."
                        });
            }

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
    AND IsDeleted = 0;";

            var user =
                await connection.QuerySingleOrDefaultAsync<UserDto>(
                    sql,
                    new
                    {
                        Id = userId
                    });

            if (user is null)
            {
                return ApiResponse<UserDto>
                    .FailureResponse(
                        new()
                        {
                            "User not found."
                        });
            }

            return ApiResponse<UserDto>
                .SuccessResponse(user);
        }
    }
}