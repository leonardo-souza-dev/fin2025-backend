using System.Security.Claims;
using Fin.Application.Constants;
using Fin.Application.Services;
using Fin.Domain.Entities;
using Fin.Domain.Exceptions;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace Fin.Application.UseCases.Auth
{
    public class RegisterUseCase(
        IUserRepository repository, 
        IUnitOfWork unitOfWork,
        IAuthService authService)
    {
        public RegisterResponse Handle(RegisterRequest request, ref IResponseCookies cookies)
        {
            if (repository.GetUserByEmail(request.Email) != null)
            {
                throw new UserAlreadExistsException(nameof(request.Email), request.Email);
            }
            
            var hashedPassword = PasswordHasher.HashPassword(request.Password);
            var newUser = new User
            {
                Email = request.Email,
                Password = hashedPassword,
                Role = "user",
                IsActive = true,
            };

            repository.Create(newUser);
            unitOfWork.SaveChanges();
            
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new(ClaimTypes.Email, newUser.Email),
                new(ClaimTypes.Role, newUser.Role)
            };

            var accessToken = authService.GenerateAccessToken(claims);
            var refreshToken = authService.GenerateRefreshToken(newUser.Id);
            
            cookies.Append(AuthConstants.RefreshTokenKey, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                //Expires = _authService.GetRefreshTokenExpiry() //TODO: fix this
                Path = "/"
            });

            return new RegisterResponse
            {
                Message = "Registration successful"
            };
        }
    }
    
    public class RegisterRequest
    {
        public required string Email { get; init; } = string.Empty;
        public required string Password { get; init; } = string.Empty;
    }


    public class RegisterResponse
    {
        public required string Message { get; init; }
    }
}