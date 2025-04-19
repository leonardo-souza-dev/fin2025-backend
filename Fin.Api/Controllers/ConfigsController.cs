using Fin.Api.Models;
using Fin.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigsController(ConfigService service) : ControllerBase
{
    private readonly ConfigService _service = service;

    [HttpGet]
    [Authorize]
    public IActionResult GetAll() => Ok(_service.GetAllActive());

    [HttpPut]
    [Authorize]
    public IActionResult Upsert([FromBody] Config config)
    {
        if (config == null)
        {
            return BadRequest("Config cannot be null");
        }
        _service.Upsert(config, out bool isCreate);

        if (isCreate)
        {
            return Created("/api/configs", config);
        }
        return Ok(config);
    }
}
