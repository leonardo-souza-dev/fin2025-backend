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
    public IActionResult Create([FromBody] CreateTransferRequest request)
    {
        var response = createTransferUseCase.Handle(request);
        return CreatedAtAction(nameof(Create), response);
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize]
    public IActionResult Edit([FromRoute] int id, [FromBody] EditTransferRequest request)
    {
        request.Id = id;
        editTransferUseCase.Handle(request);
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
