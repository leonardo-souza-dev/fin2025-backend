using Fin.Application.UseCases.Payments;
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
    [ProducesResponseType<CreatePaymentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Create([FromBody] CreatePaymentRequest request)
    {
        return CreatedAtAction(nameof(Create), createPaymentUseCase.Handle(request));
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Update([FromRoute] int id, [FromBody] EditPaymentRequest request)
    {
        request.Id = id;
        editPaymentUseCase.Handle(request);
        return Ok();
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Delete([FromRoute] int id)
    {
        deletePaymentOrRelatedTransferIfAnyUseCase.Handle(id);
        return NoContent();
    }
}
