using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fin.Api.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Email é obrigatório.")]
    [MaxLength(50)]
    public string Email { get; set; }

    [Required(ErrorMessage = "Email é obrigatório.")]
    [MaxLength(50)]
    public string Password { get; set; }

    //public string? RefreshToken { get; set; }
    public string Role { get; set; }
    //public DateTime? RefreshTokenExpiry { get; set; }
    public bool IsActive { get; set; }
}