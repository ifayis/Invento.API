namespace Invento.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }

        Guid TenantId { get; }

        string Email { get; }

        string Role { get; }
    }
}