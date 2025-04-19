namespace Fin.Api.Models;

public class Transfer
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public required string Description { get; set; }
    public int SourceAccountId { get; set; }
    public decimal Amount { get; set; }
    public int DestinationAccountId { get; set; }
    public bool IsRecurrent { get; set; }
    public bool IsActive { get; set; }
}
