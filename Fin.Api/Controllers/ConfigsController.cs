using Fin.Api.Services;
using Fin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigsController(ConfigService service) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult GetAll() => Ok(service.GetAll());

    [HttpPost]
    [Authorize]
    public IActionResult Create([FromBody] Config config)
    {
        if (config == null || config.Id.HasValue)
        {
            return BadRequest("Config cannot be null or some ID was found.");
        }

        try
        {
            service.Create(config);
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(ex);
        }

        return CreatedAtAction(nameof(Create), config);
    }

    [HttpPut]
    [Authorize]
    public IActionResult Update([FromBody] Config config)
    {
        if (config == null || !config.Id.HasValue)
        {
            return BadRequest("Config cannot be null or ID not found.");
        }

        try
        {
            service.Update(config);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest("Entity does not exists. " + ex);
        }

        return Ok(config);
    }
}
