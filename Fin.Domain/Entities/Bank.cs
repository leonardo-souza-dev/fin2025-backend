namespace Fin.Domain.Entities;

public record Bank(
    int Id, 
    string Name, 
    bool IsActive
);
