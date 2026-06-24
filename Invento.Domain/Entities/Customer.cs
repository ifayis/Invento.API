using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class Customer : AuditableEntity
    {
        public string Name { get; set; }
            = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public ICollection<Sale> Sales { get; set; } = new List<Sale>();

        public ICollection<CustomerPayment> Payments { get; set; } = new List<CustomerPayment>();
    }
}