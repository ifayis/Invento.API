public class StockMovementDto
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; }
        = string.Empty;

    public int Quantity { get; set; }

    public string MovementType { get; set; }
        = string.Empty;

    public int CurrentStockAfterMovement { get; set; }

    public string? Remarks { get; set; }

    public string? ReferenceNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }
}