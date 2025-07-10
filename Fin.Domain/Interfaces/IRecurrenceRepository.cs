using Fin.Domain.Entities;

namespace Fin.Domain.Interfaces;

public interface IRecurrenceRepository
{
    IEnumerable<Recurrence> GetAll();
}
