using Fin.Api.DTO;
using Fin.Api.Models;
using Fin.Api.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Fin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly UserService _userService;

    public AuthController(AuthService tokenService, UserService userService)
    {
        _authService = tokenService;
        _userService = userService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _userService.GetUserByEmail(request.Email);
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
            new(ClaimTypes.Role, user.Role) // roles
        };

        var accessToken = _authService.GenerateAccessToken(claims);
        var refreshToken = _authService.GenerateRefreshToken(user.Id);

        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth/refresh"
        });
        return Ok(new { accessToken });
    }

    [HttpPost("refresh")]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    {
        // Lê o refresh token do cookie
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(new { message = "Not authorized" });
        }


        var accessToken = request.AccessToken;


        var principal = _authService.GetPrincipalFromExpiredToken(accessToken);
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var user = _userService.GetUserById(int.Parse(userId!));
        if (user == null)
        {
            return BadRequest("User not found");
        }

        var newRefreshToken = _authService.GenerateRefreshToken(user.Id);
        Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = _authService.GetRefreshTokenExpiry()
        });

        var newAccessToken = _authService.GenerateAccessToken(principal.Claims);
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

            if (_userService.GetUserByEmail(request.Email) != null)
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

            _userService.Upsert(newUser);

            // 5. Gerar tokens (opcional)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new Claim(ClaimTypes.Email, newUser.Email),
                new Claim(ClaimTypes.Role, newUser.Role)
            };

            var accessToken = _authService.GenerateAccessToken(claims);
            var refreshToken = _authService.GenerateRefreshToken(newUser.Id);

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
        Response.Cookies.Delete("refreshToken");
        return Ok(new { Message = "Logout success" });
    }
}