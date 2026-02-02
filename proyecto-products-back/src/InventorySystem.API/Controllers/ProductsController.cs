using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventorySystem.Application.DTOs;
using InventorySystem.Application.Features.Products.Commands;
using InventorySystem.Application.Features.Products.Queries;

namespace InventorySystem.API.Controllers;

[Authorize]
public class ProductsController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(Result<PaginatedResult<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(string id)
    {
        var result = await Mediator.Send(new GetProductByIdQuery(id));
        return !result.IsSuccess ? NotFound(result) : Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(Result<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var result = await Mediator.Send(command);
        return !result.IsSuccess ? BadRequest(result) : CreatedAtAction(nameof(GetProduct), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(Result<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProduct(string id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest(Result<ProductDto>.Failure("El ID no coincide"));
            
        var result = await Mediator.Send(command);
        return !result.IsSuccess 
            ? (result.Message.Contains("no encontrado") ? NotFound(result) : BadRequest(result)) 
            : Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        var result = await Mediator.Send(new DeleteProductCommand(id));
        return !result.IsSuccess ? NotFound(result) : Ok(result);
    }

    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(Result<IEnumerable<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLowStockProducts([FromQuery] int threshold = 5)
    {
        return Ok(await Mediator.Send(new GetLowStockProductsQuery(threshold)));
    }

    [HttpPost("{id}/report-low-stock")]
    [Authorize(Policy = "EmployeeOrAdmin")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReportLowStock(string id)
    {
        var result = await Mediator.Send(new ReportLowStockCommand(id));
        return !result.IsSuccess ? NotFound(result) : Ok(result);
    }
}
