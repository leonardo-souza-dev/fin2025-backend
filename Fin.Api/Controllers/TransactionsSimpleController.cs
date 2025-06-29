using Fin.Api.DTO;
using Fin.Api.Models;
using Fin.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/transactions/simple")]
public class TransactionsSimpleController(TransactionService service) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public IActionResult Create([FromBody] TransactionRequest request)
    {
        if (request == null)
        {
            return BadRequest("Transaction cannot be null");
        }
        if (request.Id != null)
        {
            return BadRequest("Transaction ID must be null for creation");
        }

        var transaction = service.CreateSimple(request);

        return CreatedAtAction(nameof(Create), new { id = transaction.Id }, transaction);
    }

    [HttpPut]
    [Authorize]
    public IActionResult Update([FromBody] Transaction request)
    {
        if (request == null)
        {
            return BadRequest("Transaction cannot be null");
        }
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
