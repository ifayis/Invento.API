using Invento.Application.Features.Sales.DTOs;
using Invento.Domain.Entities;

namespace Invento.Application.Features.Sales.Extensions
{
    public static class SaleMappingExtensions
    {
        public static SaleDetailsDto ToSaleDetailsDto(this Sale sale)
        {
            return new SaleDetailsDto
            {
                Id = sale.Id,
                CustomerId = sale.CustomerId,
                InvoiceNumber = sale.InvoiceNumber,
                SaleDate = sale.SaleDate,
                SubTotal = sale.SubTotal,
                TaxAmount = sale.TaxAmount,
                DiscountAmount = sale.DiscountAmount,
                TotalAmount = sale.TotalAmount,
                ProfitAmount = sale.ProfitAmount,
                IsDeleted = sale.IsDeleted,

                Items = sale.SaleItems
                    .Select(x => new SaleItemDto
                    {
                        ProductId = x.ProductId,
                        ProductName =
                            x.Product?.Name
                            ?? string.Empty,

                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice,
                        TaxAmount = x.TaxAmount,
                        TotalPrice = x.TotalPrice,
                        GrossProfit = x.ProfitAmount
                    })
                    .ToList()
            };
        }


        public static SaleDto ToSaleDto(this Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                CustomerId = sale.CustomerId,
                InvoiceNumber = sale.InvoiceNumber,
                SaleDate = sale.SaleDate,
                TotalAmount = sale.TotalAmount,
                ProfitAmount = sale.ProfitAmount,
                IsDeleted = sale.IsDeleted
            };
        }
    }
}