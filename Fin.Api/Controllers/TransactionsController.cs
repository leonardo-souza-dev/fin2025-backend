using Fin.Application.UseCases;
using Fin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(TransactionService service) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public IActionResult Create([FromBody] TransactionRequest request)
    {
        var transaction = service.CreateSimple(request);

        return CreatedAtAction(nameof(Create), new { id = transaction.Id }, transaction);
    }

    [HttpPut]
    [Authorize]
    public IActionResult Update([FromBody] Transaction request)
    {
        var transaction = service.UpdateSimple(request);
        return Ok(transaction);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete([FromRoute] int id)
    {
        service.Delete(id);

        return NoContent();
    }
}
