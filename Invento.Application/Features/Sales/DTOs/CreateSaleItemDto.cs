namespace Invento.Application.Features.Sales.DTOs
{
    public class CreateSaleItemDto
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
