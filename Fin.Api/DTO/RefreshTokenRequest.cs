using System.ComponentModel.DataAnnotations;

namespace Fin.Api.DTO;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "AccessToken is required.")]

    public string AccessToken { get; set; } = string.Empty;
}
