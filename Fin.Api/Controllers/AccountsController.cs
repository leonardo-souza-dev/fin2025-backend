using Fin.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IAccountRepository repository) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult GetAll() => Ok(repository.GetAll());
}
