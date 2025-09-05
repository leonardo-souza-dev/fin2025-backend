using System.Security.Authentication;
using Fin.Domain.Exceptions;
using System.Security.Claims;
using Fin.Application.Constants;
using Fin.Application.Services;
using Fin.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace Fin.Application.UseCases.Auth
{
    public class RefreshTokenUseCase(IAuthService authService, IUserRepository userRepository)
    {
        public RefreshTokenResponse Handle(
            RefreshTokenRequest request, 
            ref IRequestCookieCollection cookies,
            ref IResponseCookies responseCookies)
        {
            if (!cookies.TryGetValue(AuthConstants.RefreshTokenKey, out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new AuthenticationException("Not authorized.");
            }
            
            var claimsPrincipal = authService.GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                throw new UserNotFoundException();
            }

            var user = userRepository.Get(int.Parse(userId));
            if (user == null)
            {
                throw new UserNotFoundException(nameof(userId), userId);
            }

            var newRefreshToken = authService.GenerateRefreshToken(user.Id);
            var newAccessToken = authService.GenerateAccessToken(claimsPrincipal.Claims);

            responseCookies.Append(AuthConstants.RefreshTokenKey, newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                //Expires = _authService.GetRefreshTokenExpiry()
                Path = "/"
            });
            
            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }

    public class RefreshTokenRequest
    {
        public required string AccessToken { get; set; }
    }

    public class RefreshTokenResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}