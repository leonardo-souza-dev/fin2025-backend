using Fin.Api.Repository;
using Fin.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecurrenceController(RecurrenceService service) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult GetAll() => Ok(service.GetRecurrenceMessages());
}
