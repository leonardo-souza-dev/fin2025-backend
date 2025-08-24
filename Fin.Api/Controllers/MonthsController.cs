using Fin.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonthsController(GetMonthUseCase useCase) : ControllerBase
{
    [HttpGet("year/{year}/month/{month}/accounts/{accountIds}")]
    [Authorize]
    public IActionResult Get(int year, int month, string accountIds)
    {
        var response = useCase.Handle(year, month, accountIds);
        return Ok(response);
    }
}
