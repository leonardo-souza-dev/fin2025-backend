using Fin.Domain.Entities;
using Fin.Infrastructure.Data;

namespace Fin.Infrastructure.Repositories
{
    public interface IRecurrenceRepository
    {
        IEnumerable<Recurrence> GetAll();
    }

    public class RecurrenceRepository(FinDbContext context) : IRecurrenceRepository
    {
        public IEnumerable<Recurrence> GetAll()
        {
            return [.. context.Recurrences];
        }
    }
}