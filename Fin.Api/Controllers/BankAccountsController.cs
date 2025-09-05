using Fin.Application.UseCases.BankAccounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankAccountsController(
    GetAllBankAccountsUseCase getAllBankAccountsUseCase,
    GetAllAccountsUseCase getAllAccountsUseCase) : ControllerBase
{
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var response = getAllBankAccountsUseCase.Handle();
        return Ok(response);
    }
    
    [HttpGet("accounts")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAllAccounts()
    {
        var response = getAllAccountsUseCase.Handle();
        return Ok(response);
    }
}
