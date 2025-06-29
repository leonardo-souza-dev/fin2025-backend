namespace Fin.Api.DTO;

public sealed class TransactionRequest
{
    public int? Id { get; set; }
    public required DateOnly Date { get; set; }
    public required string Description { get; set; }
    public required int FromAccountId { get; set; }
    public required decimal Amount { get; set; }
    public required int? ToAccountId {  get; set; }
    public int? RecurrenceId { get; set; }

    public int? RecurrenceEndMonth { get; set; }
    public int? RecurrenceEndYear { get; set; }
}
