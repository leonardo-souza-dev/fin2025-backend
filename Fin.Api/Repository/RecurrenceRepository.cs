using Fin.Api.Data;
using Fin.Domain.Entities;
using Fin.Domain.Interfaces;

namespace Fin.Api.Repository;

public class RecurrenceRepository(FinDbContext context) : IRecurrenceRepository
{
    public IEnumerable<Recurrence> GetAll()
    {
        return [.. context.Recurrences];
    }
}

