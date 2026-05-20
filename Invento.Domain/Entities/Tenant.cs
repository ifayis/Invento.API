using Invento.Shared.Common;

namespace Invento.Domain.Entities;

public class Tenant : BaseEntity
{
    public string CompanyName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? LogoUrl { get; set; }

    public string? TaxNumber { get; set; }

    public string? Website { get; set; }

    public bool IsDeleted { get; set; } = false;

    public ICollection<User> Users { get; set; } = new List<User>();
}