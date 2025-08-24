using Fin.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransfersController(
    TransactionService service, 
    CreateTransferUseCase createTransferUseCase,
    UpdateTransferUseCase updateTransferUseCase) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public IActionResult Create([FromBody] CreateTransferRequest request)
    {
        createTransferUseCase.Handle(request);
        return Created();
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdateTransferRequest request)
    {
        request.Id = id;
        updateTransferUseCase.Handle(request);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public IActionResult DeleteTransfer([FromRoute] int id)
    {
        service.Delete(id);

        return NoContent();
    }
}
