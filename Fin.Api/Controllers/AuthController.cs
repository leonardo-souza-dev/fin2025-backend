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
    private const string REFRESH_TOKEN_KEY = "refreshToken";

    [HttpPost("login")]
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
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var cookies = Response.Cookies;
        var response = registerUseCase.Handle(request, ref cookies);

        return Created("/", response);
    }

    [HttpDelete("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(REFRESH_TOKEN_KEY);
        return Ok(new { Message = "Logout success" });
    }
}