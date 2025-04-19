namespace Fin.Api.Models;

public record Account(
    int Id, 
    string Name, 
    int BankId,
    string? Comments,
    bool IsActive
);
