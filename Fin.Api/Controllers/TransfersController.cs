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
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult Create([FromBody] CreateTransferRequest request)
    {
        var response = createTransferUseCase.Handle(request);
        return CreatedAtAction(nameof(Create), response);
    }

    [Authorize]
    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Edit([FromRoute] int id, [FromBody] EditTransferRequest request)
    {
        request.Id = id;
        editTransferUseCase.Handle(request);
        return Ok();
    }

    [Authorize]
    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete([FromRoute] int id)
    {
        deleteTransferUseCase.Handle(id);
        return NoContent();
    }
}
