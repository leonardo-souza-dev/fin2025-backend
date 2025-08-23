using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fin.Domain.Entities;

public class Transfer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required int FromTransactionId { get; set; }
    public required int ToTransactionId { get; set; }
    public bool IsActive { get; set; }
}
