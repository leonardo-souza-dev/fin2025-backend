using Fin.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Fin.Api.DTO;
using Fin.Application.UseCases;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService, 
    UserService userService,
    LoginUseCase loginUseCase,
    RefreshTokenUseCase refreshTokenUseCase
    ) : ControllerBase
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
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userService.GetUserByEmail(request.Email) != null)
            {
                return Conflict("Email already exists.");
            }

            var hashedPassword = PasswordHasher.HashPassword(request.Password);
            var newUser = new User
            {
                Email = request.Email,
                Password = hashedPassword,
                Role = "user",
                IsActive = true,
            };

            userService.Create(newUser);

            // 5. Gerar tokens (opcional)
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new(ClaimTypes.Email, newUser.Email),
                new(ClaimTypes.Role, newUser.Role)
            };

            var accessToken = authService.GenerateAccessToken(claims);
            var refreshToken = authService.GenerateRefreshToken(newUser.Id);

            Response.Cookies.Append(REFRESH_TOKEN_KEY, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                //Expires = _authService.GetRefreshTokenExpiry()
                Path = "/"
            });

            return Created("/", new { message = "User registered." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocorreu um erro interno. {ex}");
        }
    }

    [HttpDelete("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(REFRESH_TOKEN_KEY);
        return Ok(new { Message = "Logout success" });
    }
}