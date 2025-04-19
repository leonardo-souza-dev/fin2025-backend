using System.ComponentModel.DataAnnotations;

namespace Fin.Api.DTO;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "AccessToken é obrigatório.")]

    public string AccessToken { get; set; } = string.Empty;

    //[Required(ErrorMessage = "RefreshToken é obrigatório.")]

    public string? RefreshToken { get; set; } = string.Empty;
}
