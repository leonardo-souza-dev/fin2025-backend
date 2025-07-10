using Fin.Api.Services;
using Fin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/transactions/transfer")]
public class TransactionsTransferController(TransactionService service) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public IActionResult Create([FromBody] Transaction request)
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

        return CreatedAtAction(nameof(Create), transactionsTransfer);
    }

    [HttpPut]
    [Authorize]
    public IActionResult Update([FromBody] Transaction request)
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

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult DeleteTransfer([FromRoute] int id)
    {
        service.Delete(id);

        return NoContent();
    }
}
