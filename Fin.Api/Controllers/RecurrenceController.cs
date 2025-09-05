using Fin.Application.UseCases;
using Fin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecurrenceController(RecurrenceService service) : ControllerBase
{
    [HttpGet]
    [Authorize]
    [ProducesResponseType<List<RecurrenceMessage>>(StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(service.GetRecurrenceMessages());
}
