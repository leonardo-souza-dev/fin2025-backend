using Microsoft.EntityFrameworkCore;

namespace Fin.Api.Models;

public class Transfer
{
    public required int FromTransactionId { get; set; }
    public required int ToTransactionId { get; set; }

    public Transaction FromTransaction { get; set; } = null!;
    public Transaction ToTransaction { get; set; } = null!;
}
