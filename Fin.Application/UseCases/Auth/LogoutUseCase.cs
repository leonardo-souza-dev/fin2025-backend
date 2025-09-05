using Microsoft.AspNetCore.Http;

namespace Fin.Application.UseCases.Auth
{
    public class LogoutUseCase
    {
        private const string REFRESH_TOKEN_KEY = "refreshToken"; //TODO: refactor
        
        public LogoutResponse Handle(ref IResponseCookies responseCookies)
        {
            responseCookies.Delete(REFRESH_TOKEN_KEY);

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