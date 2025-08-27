using Fin.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController(
    CreatePaymentUseCase createPaymentUseCase,
    EditPaymentUseCase editPaymentUseCase,
    DeletePaymentOrRelatedTransferIfAnyUseCase deletePaymentOrRelatedTransferIfAnyUseCase) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public IActionResult Create([FromBody] CreatePaymentRequest request)
    {
        return CreatedAtAction(nameof(Create), createPaymentUseCase.Handle(request));
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize]
    public IActionResult Update([FromRoute] int id, [FromBody] EditPaymentRequest request)
    {
        request.Id = id;
        editPaymentUseCase.Handle(request);
        return Ok();
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize]
    public IActionResult Delete([FromRoute] int id)
    {
        deletePaymentOrRelatedTransferIfAnyUseCase.Handle(id);
        return NoContent();
    }
}
