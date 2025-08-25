using Fin.Application.UseCases;
using Fin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController(
    CreatePaymentUseCase createPaymentUseCase,
    UpdatePaymentUseCase updatePaymentUseCase,
    DeletePaymentOrTransferUseCase deletePaymentOrTransferUseCase) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public IActionResult Create([FromBody] CreatePaymentRequest request) =>
        CreatedAtAction(nameof(Create), createPaymentUseCase.Handle(request));

    [HttpPut("{id:int:min(1)}")]
    [Authorize]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdatePaymentRequest request)
    {
        request.Id = id;
        updatePaymentUseCase.Handle(request);
        return Ok();
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize]
    public IActionResult Delete([FromRoute] int id)
    {
        deletePaymentOrTransferUseCase.Handle(id);
        return NoContent();
    }
}
