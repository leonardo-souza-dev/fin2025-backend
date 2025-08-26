using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fin.Domain.Entities;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [MaxLength(100)]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [MaxLength(100)]
    public required string Password { get; set; }

    public string? Role { get; set; }
    
    public bool IsActive { get; set; }
}