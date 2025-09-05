using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fin.Domain.Entities;

public class Payment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    public required DateOnly Date { get; set; }
    public required string Description { get; set; }
    public required int FromAccountId { get; set; }
    public required decimal Amount { get; set; }
    public int? ToAccountId { get; set; }// TODO: remove toAccountId
    public int? RecurrenceId { get; set; }
    public bool IsActive { get; set; }
    public int? TransferId { get; set; }
    
    [NotMapped]
    public int? PaymentIdTransferRelated { get; set; }
}
