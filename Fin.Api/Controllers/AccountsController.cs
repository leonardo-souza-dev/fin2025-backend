using Fin.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IGetAccountsUseCase getAccountsUseCase) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult GetAll()
    {
        var response = getAccountsUseCase.Handle();
        return Ok(response);
    }
}
