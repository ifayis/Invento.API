using Invento.Shared.Common;

namespace Invento.Domain.Entities;

public class StockMovement : BaseEntity
{
    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    public string MovementType { get; set; }
        = string.Empty;

    public string? Remarks { get; set; }

    public string? ReferenceNumber { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public bool IsDeleted { get; set; } = false;
}