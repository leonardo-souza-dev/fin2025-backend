namespace Fin.Api.Models;

public class Transaction
{
    public int? Id { get; set; }
    public DateOnly Date { get; set; }
    public string Description { get; set; }
    public int RefAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Type { get; set; }
    public int? OtherAccountId { get; set; }
    public bool IsRecurrent { get; set; }
    public bool IsActive { get; set; }
}
