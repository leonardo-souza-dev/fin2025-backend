using System.Security.Authentication;
using Fin.Domain.Exceptions;
using Fin.Infrastructure.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Fin.Application.UseCases.Auth
{
    public class LoginUseCase(IUserRepository userRepository, IAuthService authService)
    {
        private const string REFRESH_TOKEN_KEY = "refreshToken";
        
        public LoginResponse Handle(LoginRequest request, ref IResponseCookies cookies)
        {
            ArgumentNullException.ThrowIfNull(request);
            
            var user = userRepository.GetUserByEmail(request.Email);
            if (user == null)
            {
                throw new UserNotFoundException(nameof(request.Email), request.Email);
            }
            
            if (!PasswordHasher.VerifyPassword(request.Password, user.Password))
            {
                throw new AuthenticationException();

            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email)/*,
                new(ClaimTypes.Role, user.Role)*/
            };
            
            var accessToken = authService.GenerateAccessToken(claims);
            var refreshToken = authService.GenerateRefreshToken(user.Id);

            cookies.Append(REFRESH_TOKEN_KEY, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }

    public sealed class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Work factor = 13 (balance entre segurança e performance)
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13); 
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
        }
    }
}