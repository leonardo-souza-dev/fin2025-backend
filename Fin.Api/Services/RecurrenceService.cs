using System.Data;
using Fin.Api.Data;
using Fin.Api.Models;

namespace Fin.Api.Services;

public class RecurrenceService(FinDbContext context)
{
    private readonly FinDbContext _context = context;

    public IEnumerable<Recurrence> GetAllActive(string yearDashMonth)
    {
        var yearMonth = yearDashMonth.Split('-');
        var year = int.Parse(yearMonth[0]);
        var month = int.Parse(yearMonth[1]);

        var recurrences = _context.Recurrences
            .Where(r => r.IsActive).ToList()
            .Where(recurrence => !string.IsNullOrEmpty(recurrence.StartYearMonth) &&
                        recurrence.GetStartYear() <= year &&
                        recurrence.GetStartMonth() <= month &&
                        !string.IsNullOrEmpty(recurrence.EndYearMonth) &&
                        recurrence.GetEndYear() >= year &&
                        recurrence.GetEndMonth() >= month);

        return recurrences;
    }
}
