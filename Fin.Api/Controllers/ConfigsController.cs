using Fin.Application.UseCases;
using Fin.Application.UseCases.Configs;
using Fin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigsController(
    ConfigService service,
    GetAllConfigsUseCase getAllConfigsUseCase,
    UpdateConfigUseCase updateConfigUseCase) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult GetAll()
    {
        var response = getAllConfigsUseCase.Handle();
        return Ok(response);
    }

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

    [HttpPut("{id:int:min(1)}")]
    [Authorize]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdateConfigRequest request)
    {
        request.Id = id;
        updateConfigUseCase.Handle(request);
        return Ok();
    }
}
