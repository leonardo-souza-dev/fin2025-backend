namespace Fin.Domain.Entities;

public class Transfer
{
    public required int FromTransactionId { get; init; }
    public Transaction FromTransaction { get; init; }
    public required int ToTransactionId { get; init; }
    public Transaction ToTransaction { get; init; }
    public required bool IsActive { get; set; }
}
