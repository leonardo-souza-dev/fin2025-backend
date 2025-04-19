namespace Fin.Api.Models;

public class Recurrence
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string StartYearMonth { get; set; }
    public string EndYearMonth { get; set; }
    public short Day { get; set; }
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public bool IsActive { get; set; }

    public int GetStartYear()
    {
        return int.Parse(StartYearMonth.Substring(0, 4));
    }

    public int GetStartMonth()
    {
        return int.Parse(StartYearMonth.Substring(5, 2));
    }

    public int GetEndYear()
    {
        return int.Parse(EndYearMonth.Substring(0, 4));
    }

    public int GetEndMonth()
    {
        return int.Parse(EndYearMonth.Substring(5, 2));
    }
}