using Fin.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransfersController(
    CreateTransferUseCase createTransferUseCase,
    UpdateTransferUseCase updateTransferUseCase,
    DeleteTransferUseCase deleteTransferUseCase) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public IActionResult Create([FromBody] CreateTransferRequest request)
    {
        var response = createTransferUseCase.Handle(request);
        return CreatedAtAction(nameof(Create), response);
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdateTransferRequest request)
    {
        request.Id = id;
        updateTransferUseCase.Handle(request);
        return Ok();
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize]
    public IActionResult Delete([FromRoute] int id)
    {
        deleteTransferUseCase.Handle(id);
        return NoContent();
    }
}
