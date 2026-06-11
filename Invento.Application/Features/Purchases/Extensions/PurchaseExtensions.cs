using Invento.Application.Features.Purchases.DTOs;
using Invento.Domain.Entities;

namespace Invento.Application.Features.Purchases.Extensions
{
    public static class PurchaseExtensions
    {
        public static PurchaseDto ToPurchaseDto(
            this Purchase purchase)
        {
            return new PurchaseDto
            {
                Id = purchase.Id,
                SupplierId = purchase.SupplierId,

                SupplierName =
                    purchase.Supplier?.Name
                    ?? string.Empty,

                PurchaseNumber = purchase.PurchaseNumber,
                PurchaseDate = purchase.PurchaseDate,
                TotalAmount = purchase.TotalAmount,
                IsDeleted = purchase.IsDeleted
            };
        }
        public static PurchaseDetailsDto ToPurchaseDetailsDto(
            this Purchase purchase)
        {
            return new PurchaseDetailsDto
            {
                Id = purchase.Id,
                SupplierId = purchase.SupplierId,

                SupplierName =
                    purchase.Supplier?.Name
                    ?? string.Empty,

                PurchaseNumber = purchase.PurchaseNumber,
                PurchaseDate = purchase.PurchaseDate,

                SubTotal = purchase.SubTotal,
                TaxAmount = purchase.TaxAmount,
                DiscountAmount = purchase.DiscountAmount,
                TotalAmount = purchase.TotalAmount,

                IsDeleted = purchase.IsDeleted,

                Items = purchase.PurchaseItems
                    .Select(x => new PurchaseItemDto
                    {
                        ProductId = x.ProductId,

                        ProductName =
                            x.Product?.Name
                            ?? string.Empty,

                        Quantity = x.Quantity,
                        UnitCost = x.UnitCost,
                        TaxAmount = x.TaxAmount,
                        TotalPrice = x.TotalPrice
                    })
                    .ToList()
            };
        }
    }
}