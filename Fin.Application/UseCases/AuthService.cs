using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Fin.Application.UseCases;

public class AuthService(IConfiguration configuration)
{
    private const string FIN2025_JWT_SECRET_KEY = "FIN2025_JWT_SECRET_KEY";

    private string GetSecretKey()
    {
        var secretKey = Environment.GetEnvironmentVariable(FIN2025_JWT_SECRET_KEY);

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException(
                "JWT Secret Key não configurada. Configure a variável de ambiente JWT_SECRET_KEY ou use User Secrets.");
        }

        // Validação básica da força da chave
        if (secretKey.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT Secret Key deve ter pelo menos 32 caracteres para segurança adequada.");
        }

        return secretKey;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSecretKey()));
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
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSecretKey()));
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
        var jwtSettings = configuration.GetSection("JwtSettings");
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, // Pode ajustar conforme necessário
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSecretKey())),
            ValidateLifetime = false // Ignora expiração para pegar claims
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
        return principal;
    }

    public DateTime GetRefreshTokenExpiry()
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var refreshTokenExpiryInMinutes = Convert.ToDouble(jwtSettings["RefreshTokenExpiryInMinutes"]);
        return DateTime.UtcNow.AddMinutes(refreshTokenExpiryInMinutes);
    }
}