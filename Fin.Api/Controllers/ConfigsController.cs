using Fin.Application.UseCases;
using Fin.Application.UseCases.Configs;
using Fin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigsController(
    GetAllConfigsUseCase getAllConfigsUseCase,
    CreateConfigUseCase createConfigUseCase,
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
    public IActionResult Create([FromBody] CreateConfigRequest request)
    {
        var response = createConfigUseCase.Handle(request);
        return CreatedAtAction(nameof(Create), response);
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
