using Fin.Application.UseCases.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(GetAllAccountsUseCase getAllAllAccountsUseCase) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult GetAll()
    {
        var response = getAllAllAccountsUseCase.Handle();
        return Ok(response);
    }
}
