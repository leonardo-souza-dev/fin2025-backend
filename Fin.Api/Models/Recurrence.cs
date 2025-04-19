namespace Fin.Api.Models;

public class Recurrence
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string StartYearMonth { get; set; }
    public required string EndYearMonth { get; set; }
    public short Day { get; set; }
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public bool IsActive { get; set; }

    public int GetStartYear()
    {
        return int.Parse(StartYearMonth[..4]);
    }

    public int GetStartMonth()
    {
        return int.Parse(StartYearMonth.Substring(5, 2));
    }

    public int GetEndYear()
    {
        return int.Parse(EndYearMonth[..4]);
    }

    public int GetEndMonth()
    {
        return int.Parse(EndYearMonth.Substring(5, 2));
    }
}