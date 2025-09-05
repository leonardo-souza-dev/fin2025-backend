using Fin.Application.UseCases.Transfers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransfersController(
    CreateTransferUseCase createTransferUseCase,
    EditTransferUseCase editTransferUseCase,
    DeleteTransferUseCase deleteTransferUseCase) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Create([FromBody] CreateTransferRequest request)
    {
        var response = createTransferUseCase.Handle(request);
        return CreatedAtAction(nameof(Create), response);
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Edit([FromRoute] int id, [FromBody] EditTransferRequest request)
    {
        request.Id = id;
        editTransferUseCase.Handle(request);
        return Ok();
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Delete([FromRoute] int id)
    {
        deleteTransferUseCase.Handle(id);
        return NoContent();
    }
}
