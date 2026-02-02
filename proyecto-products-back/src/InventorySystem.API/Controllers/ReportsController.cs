using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventorySystem.Application.Features.Reports.Queries;

namespace InventorySystem.API.Controllers;

[Authorize(Policy = "AdminOnly")]
public class ReportsController : BaseApiController
{
    [HttpGet("low-stock/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLowStockReport([FromQuery] int threshold = 5)
    {
        var result = await Mediator.Send(new GenerateLowStockReportQuery(threshold));
        
        if (!result.IsSuccess || result.Data == null)
            return BadRequest(result);

        return File(result.Data, "application/pdf", $"reporte-stock-bajo-{DateTime.Now:yyyyMMdd-HHmmss}.pdf");
    }

    [HttpGet("inventory/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInventoryReport()
    {
        var result = await Mediator.Send(new GenerateInventoryReportQuery());
        
        if (!result.IsSuccess || result.Data == null)
            return BadRequest(result);

        return File(result.Data, "application/pdf", $"reporte-inventario-{DateTime.Now:yyyyMMdd-HHmmss}.pdf");
    }
}
