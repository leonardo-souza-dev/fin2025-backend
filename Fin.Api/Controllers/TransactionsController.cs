using Fin.Api.Models;
using Fin.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(TransactionService service) : ControllerBase
{
    private readonly TransactionService _service = service;

    [HttpGet]
    [Authorize]
    public IActionResult GetAll(string monthYear) => Ok(_service.GetAllActive(monthYear));

    [HttpPut]
    [Authorize]
    public IActionResult Upsert([FromBody] Transaction request)
    {
        if (request == null)
        {
            return BadRequest("Transaction cannot be null");
        }
        var transaction = _service.Upsert(request);
        return Ok(transaction);
    }

    [HttpDelete("{idType}")]
    [Authorize]
    public IActionResult Delete([FromRoute] string idType)
    {
        if (idType == null)
        {
            return BadRequest("IdType cannot be null.");
        }
        if (idType.Split("_").Length != 2)
        {
            return BadRequest("IdType is in wrong format.");
        }
        if (!int.TryParse(idType.Split("_")[0], out _))
        {
            return BadRequest("IdType is in wrong format.");
        }

        _service.Delete(idType);
        return NoContent();
    }
}
