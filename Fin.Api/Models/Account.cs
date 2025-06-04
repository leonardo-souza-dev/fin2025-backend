namespace Fin.Api.Models;

public class Account
{
    public int? Id { get; set; }
    public required string Name { get; set; }
    public required int BankId { get; set; }
    public string? Comments { get; set; }
    public required bool IsActive { get; set; }
}
