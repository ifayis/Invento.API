using Invento.Domain.Enums;

namespace Invento.Application.Features.Users.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }
            = string.Empty;

        public string Email { get; set; }
            = string.Empty;

        public UserRole Role { get; set; }

        public bool IsActive { get; set; }

        public bool MustChangePassword { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public Guid TenantId { get; set; }

        public Guid? CreatedByUserId { get; set; }
    }
}