using Fin.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(FinDbContext context) : ControllerBase
{
    private readonly FinDbContext _context = context;

    [HttpGet]
    [Authorize]
    public IActionResult GetAll() =>Ok(_context.Accounts.Where(a => a.IsActive));
}
