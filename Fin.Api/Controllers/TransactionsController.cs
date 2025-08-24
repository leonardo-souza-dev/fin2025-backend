using Fin.Application.UseCases;
using Fin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(
    CreateTransactionUseCase createTransactionUseCase,
    UpdateTransactionUseCase updateTransactionUseCase,
    TransactionService service) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public IActionResult Create([FromBody] CreateTransactionRequest request) =>
        CreatedAtAction(nameof(Create), createTransactionUseCase.Handle(request));

    [HttpPut("{id:int}")]
    [Authorize]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdateTransactionRequest request)
    {
        request.Id = id;
        updateTransactionUseCase.Handle(request);
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete([FromRoute] int id)
    {
        service.Delete(id);

        return NoContent();
    }
}
