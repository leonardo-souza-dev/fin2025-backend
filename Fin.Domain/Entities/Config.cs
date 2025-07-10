namespace Fin.Domain.Entities;

public class Config
{
    public int? Id { get; set; }

    public required string Key { get; set; }

    public required string Value { get; set; }

    public bool IsActive { get; set; }
}
