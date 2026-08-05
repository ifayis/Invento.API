namespace Invento.Application.Features.Sales.DTOs
{
    public class DeleteSaleDto
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
