namespace Fin.Api.Models;

public class Transaction
{
    public int? Id { get; set; }
    public required DateOnly Date { get; set; }
    public required string Description { get; set; }
    public required int FromAccountId { get; set; }
    public required decimal Amount { get; set; }
    public int? ToAccountId { get; set; }
    
    public int? RecurrenceId { get; set; }
    
    public DateOnly? RecurrenceStartMonth { get; set; }
    public DateOnly? RecurrenceEndMonth { get; set; }
    public int? RecurrenceDay { get; set; }

    public required bool IsActive { get; set; }

    public string Type => ToAccountId != null ? "TRANSFER" : "SIMPLE";
    public bool IsRecurrent => RecurrenceId != null;

    public Transaction CreateRelatedTransfer()
    {
        if (Type != "TRANSFER")
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
