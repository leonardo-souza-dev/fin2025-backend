using Fin.Api.Models;
using Fin.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(TransactionService service) : ControllerBase
{
    [HttpPost("simple")]
    [Authorize]
    public IActionResult CreateSimple([FromBody] Transaction request)
    {
        if (request == null)
        {
            return BadRequest("Transaction cannot be null");
        }
        if (request.Id != null)
        {
            return BadRequest("Transaction ID must be null for creation");
        }
        if (request.Type == "TRANSFER")
        {
            return BadRequest("ToAccountId must be null for non-transfer transactions");
        }

        var transaction = service.Create(request);

        return CreatedAtAction(nameof(CreateSimple), new { id = transaction.Id }, transaction);
    }

    [HttpPut("simple")]
    [Authorize]
    public IActionResult UpdateSimple([FromBody] Transaction request)
    {
        if (request == null)
        {
            return BadRequest("Transaction cannot be null");
        }
        var transaction = service.Update(request);
        return Ok(transaction);
    }

    [HttpDelete("simple/{id}")]
    [Authorize]
    public IActionResult DeleteSimple([FromRoute] int id)
    {
        service.Delete(id);

        return NoContent();
    }

    [HttpPost("transfer")]
    [Authorize]
    public IActionResult CreateTransfer([FromBody] Transaction request)
    {
        if (request == null)
        {
            return BadRequest("Transaction cannot be null");
        }
        if (request.Id != null)
        {
            return BadRequest("Transaction ID must be null for creation");
        }
        if (request.Type != "TRANSFER")
        {
            return BadRequest("ToAccountId must be provided for transfer transactions");
        }

        var transactionsTransfer = service.CreateTransfer(request);

        return CreatedAtAction(nameof(CreateTransfer), transactionsTransfer);
    }

    [HttpPut("transfer")]
    [Authorize]
    public IActionResult UpdateTransfer([FromBody] Transaction request)
    {
        if (request == null)
        {
            return BadRequest("Transaction cannot be null");
        }
        if (request.Id == null)
        {
            return BadRequest("Transaction ID must be provided for update");
        }
        if (request.Type != "TRANSFER")
        {
            return BadRequest("ToAccountId must be provided for transfer transactions");
        }

        var transactionsTransfer = service.UpdateTransfer(request);

        return Ok(transactionsTransfer);
    }

    [HttpDelete("transfer/{id}")]
    [Authorize]
    public IActionResult DeleteTransfer([FromRoute] int id)
    {
        service.Delete(id);

        return NoContent();
    }
}
