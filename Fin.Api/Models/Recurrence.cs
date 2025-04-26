namespace Fin.Api.Models;

public class Recurrence
{
    public int? Id { get; set; }
    public required string StartYearMonth { get; set; }
    public required string EndYearMonth { get; set; }
    public required int Day { get; set; }
}
