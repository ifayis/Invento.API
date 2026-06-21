using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class Supplier : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? ContactPerson { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? TaxRegistrationNumber { get; set; }

        public ICollection<Purchase> Purchases { get; set; }
            = new List<Purchase>();

        public ICollection<SupplierPayment> Payments
        { get; set; } = new List<SupplierPayment>();
    }
}