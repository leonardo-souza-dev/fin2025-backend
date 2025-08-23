using Fin.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BanksController(IBankRepository repository) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult GetAll()
    {
        var result = repository.GetAll(); 
        return Ok(result);
    }
}
