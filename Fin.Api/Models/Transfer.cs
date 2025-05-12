namespace Fin.Api.Models;

public class Transfer
{
    public required int FromTransactionId { get; set; }
    public required int ToTransactionId { get; set; }
    public required bool IsActive { get; set; }
}
