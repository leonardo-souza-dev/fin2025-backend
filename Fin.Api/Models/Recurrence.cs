namespace Fin.Api.Models;

public class Recurrence
{
    public int Id { get; set; }
    public string Frequency { get; set; }
    public string BaseDescription { get; set; }
    public int BaseDay { get; set; }
    public string StartMonthYear { get; set; }
    public string EndMonthYear { get; set; }
}
