using Fin.Api.Models;
using Fin.Api.Repository;

namespace Fin.Api.Services;

public class RecurrenceService(IRecurrenceRepository repository, ITransactionRepository transactionRepository)
{
    public List<RecurrenceMessage> GetRecurrenceMessages()
    {
        var recurrences = repository.GetAll();
        var recurrentTransactions = transactionRepository.GetAll().Where(t => 
            t.IsActive && t.RecurrenceId != null && 
            recurrences.Any(r => r.Id == t.RecurrenceId)
        );

        return recurrences.Select(r => new RecurrenceMessage
        {
            Message = $""
        }).ToList();
    }
}
