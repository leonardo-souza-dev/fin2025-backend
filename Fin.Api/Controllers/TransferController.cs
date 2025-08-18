using Fin.Api.Services;
using Fin.Application.Interfaces;
using Fin.Application.UseCases;
using Fin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/transfer")]
public class TransferController(TransactionService service, ICreateTransferHandler createTransferHandler) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] Transaction request)
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

        //var transactionsTransfer = service.CreateTransfer(request);

        var createTransferRequest = new CreateTransferRequest
        {
            Date = request.Date,
            Description = request.Description,
            Amount = request.Amount,
            FromAccountId = request.FromAccountId,
            ToAccountId = request.ToAccountId.Value,
            IsRecurrence = request.IsRecurrent
        };
        var transfer = await createTransferHandler.Handle(createTransferRequest);

        return CreatedAtAction(nameof(Create), transfer);
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
