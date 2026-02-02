using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventorySystem.Application.DTOs;
using InventorySystem.Application.Features.Categories.Commands;
using InventorySystem.Application.Features.Categories.Queries;

namespace InventorySystem.API.Controllers;

[Authorize]
public class CategoriesController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(Result<IEnumerable<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories()
    {
        return Ok(await Mediator.Send(new GetCategoriesQuery()));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategory(string id)
    {
        var result = await Mediator.Send(new GetCategoryByIdQuery(id));
        return !result.IsSuccess ? NotFound(result) : Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(Result<CategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var result = await Mediator.Send(command);
        return !result.IsSuccess ? BadRequest(result) : CreatedAtAction(nameof(GetCategory), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(Result<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(string id, [FromBody] UpdateCategoryCommand command)
    {
        if (id != command.Id)
            return BadRequest(Result<CategoryDto>.Failure("El ID no coincide"));
            
        var result = await Mediator.Send(command);
        return !result.IsSuccess ? NotFound(result) : Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        var result = await Mediator.Send(new DeleteCategoryCommand(id));
        return !result.IsSuccess ? NotFound(result) : Ok(result);
    }
}
