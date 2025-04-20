namespace Fin.Api.Models;

public class Transaction2
{
    public int? Id { get; set; }
    public DateOnly Date { get; set; }
    public required string Description { get; set; }
    public int RefAccountId { get; set; }
    public decimal Amount { get; set; }
    public int? OtherAccountId { get; set; }
    public int? RecurrenceId { get; set; }
    public int? TransferId { get; set; }
    public bool IsActive { get; set; }

    public bool IsSimple => OtherAccountId == null;
    public bool IsTransfer => OtherAccountId != null;
    public bool IsRecurrent => RecurrenceId != null;
}
