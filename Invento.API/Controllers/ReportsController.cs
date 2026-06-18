using Invento.Application.Features.Reports.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invento.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(
        IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("sales-summary")]
        public async Task<IActionResult> SalesSummary(
            [FromQuery] GetSalesSummaryReportQuery query)
        {
            return Ok(
                await _mediator.Send(query));
        }

        [HttpGet("purchase-summary")]
        public async Task<IActionResult> PurchaseSummary(
            [FromQuery] GetPurchaseSummaryReportQuery query)
        {
            return Ok(
                await _mediator.Send(query));
        }

        [HttpGet("inventory-summary")]
        public async Task<IActionResult> InventorySummary()
        {
            return Ok(
                await _mediator.Send(
                    new GetInventorySummaryReportQuery()));
        }

        [HttpGet("low-stock-products")]
        public async Task<IActionResult> LowStockProducts()
        {
            return Ok(
                await _mediator.Send(
                    new GetLowStockProductsQuery()));
        }

        [HttpGet("stock-movements")]
        public async Task<IActionResult> StockMovements(
            [FromQuery] GetStockMovementHistoryQuery query)
        {
            return Ok(
                await _mediator.Send(query));
        }
    }

}
