using Microsoft.AspNetCore.Http;
using Fin.Application.Constants;

namespace Fin.Application.UseCases.Auth
{
    public class LogoutUseCase
    {
        public LogoutResponse Handle(ref IResponseCookies responseCookies)
        {
            responseCookies.Delete(AuthConstants.RefreshTokenKey);

            return LogoutResponse.Of();
        }
    }

    public class LogoutResponse
    {
        public required string Message { get; set; }

        public static LogoutResponse Of()
        {
            return new LogoutResponse
            {
                Message = "Logout success"
            };
        }
    }
}