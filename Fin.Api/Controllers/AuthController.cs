using Microsoft.AspNetCore.Mvc;
using Fin.Application.UseCases.Auth;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    LoginUseCase loginUseCase,
    RefreshTokenUseCase refreshTokenUseCase,
    RegisterUseCase registerUseCase) : ControllerBase
{
    private const string REFRESH_TOKEN_KEY = "refreshToken"; //TODO: refactor

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var cookies = Response.Cookies;
        var loginResponse = loginUseCase.Handle(request, ref cookies);
        
        return Ok(new LoginResponse
        {
            AccessToken = loginResponse.AccessToken,
            RefreshToken = loginResponse.RefreshToken
        });
    }

    [HttpPost("refresh")]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var requestCookies = Request.Cookies;
        var responseCookies = Response.Cookies;
        var response = refreshTokenUseCase.Handle(request, ref requestCookies, ref responseCookies);
        
        return Ok(response);
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var cookies = Response.Cookies;
        var response = registerUseCase.Handle(request, ref cookies);

        return CreatedAtAction(nameof(Register), response);
    }

    [HttpDelete("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(REFRESH_TOKEN_KEY);
        return Ok(new { Message = "Logout success" });
    }
}