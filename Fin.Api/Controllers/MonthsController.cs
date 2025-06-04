using Fin.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonthsController(MonthService service) : ControllerBase
{
    [HttpGet("year/{year}/month/{month}/accounts/{accountIds}")]
    [Authorize]
    public IActionResult Get(int year, int month, string accountIds)
    {
        var monthObj = service.Get(year, month, accountIds);
        return Ok(monthObj);
    }
}
