using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fin.Api.Services;

public class AuthService(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpiryInMinutes"])),
            signingCredentials: signinCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    public string GenerateRefreshToken(int userId)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokenOptions = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            },
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["RefreshTokenExpiryInMinutes"])),
            signingCredentials: signinCredentials
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, // Pode ajustar conforme necessário
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
            ValidateLifetime = false // Ignora expiração para pegar claims
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
        return principal;
    }

    public DateTime GetRefreshTokenExpiry()
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var refreshTokenExpiryInMinutes = Convert.ToDouble(jwtSettings["RefreshTokenExpiryInMinutes"]);
        return DateTime.UtcNow.AddMinutes(refreshTokenExpiryInMinutes);
    }
}