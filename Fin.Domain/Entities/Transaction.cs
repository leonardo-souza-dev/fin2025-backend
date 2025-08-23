using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fin.Domain.Entities;

public class Transaction
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
    public int? TransactionIdTransferRelated { get; set; }
    
    public string GetTransactionType() => ToAccountId != null ? "TRANSFER" : "SIMPLE";
    public bool IsRecurrent() => RecurrenceId != null;

    public Transaction CreateRelatedTransfer()
    {
        if (this.GetTransactionType() != "TRANSFER")
        {
            throw new InvalidOperationException("Cannot invert accounts for a non-transfer transaction.");
        }
        return new Transaction
        {
            Date = Date,
            Description = Description,
            FromAccountId = ToAccountId!.Value,
            Amount = Amount * -1,
            ToAccountId = FromAccountId,
            RecurrenceId = RecurrenceId,
            IsActive = IsActive
        };
    }
}
