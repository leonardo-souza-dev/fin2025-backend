using Fin.Api.DTO;
using Fin.Api.Models;
using Fin.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    AuthService authService, 
    UserService userService,
    IWebHostEnvironment webHostEnvironment
    ) : ControllerBase
{
    private const string REFRESH_TOKEN_KEY = "refreshToken";

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = userService.GetUserByEmail(request.Email);
        if (user == null)
        {
            return Unauthorized("User not found.");
        }
        if (!PasswordHasher.VerifyPassword(request.Password, user.Password))
        {
            return Unauthorized("Invalid credentials");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var accessToken = authService.GenerateAccessToken(claims);
        var refreshToken = authService.GenerateRefreshToken(user.Id);

        Response.Cookies.Append(REFRESH_TOKEN_KEY, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });
        return Ok(new { accessToken });
    }

    [HttpPost("refresh")]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (!Request.Cookies.TryGetValue(REFRESH_TOKEN_KEY, out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(new { message = "Not authorized" });
        }

        var accessToken = request.AccessToken;

        var principal = authService.GetPrincipalFromExpiredToken(accessToken);
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var user = userService.GetUserById(int.Parse(userId!));
        if (user == null)
        {
            return BadRequest("User not found");
        }

        var newRefreshToken = authService.GenerateRefreshToken(user.Id);
        Response.Cookies.Append(REFRESH_TOKEN_KEY, newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            //Expires = _authService.GetRefreshTokenExpiry()
            Path = "/"
        });

        var newAccessToken = authService.GenerateAccessToken(principal.Claims);
        return Ok(new { AccessToken = newAccessToken });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] DTO.RegisterRequest request)
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

            userService.Upsert(newUser);

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