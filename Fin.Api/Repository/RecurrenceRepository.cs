using Fin.Api.Data;
using Fin.Api.Models;

namespace Fin.Api.Repository;

public class RecurrenceRepository(FinDbContext context) : IRecurrenceRepository
{
    public IEnumerable<Recurrence> GetAll()
    {
        return [.. context.Recurrences];
    }
}

public interface IRecurrenceRepository
{
    IEnumerable<Recurrence> GetAll();
}
