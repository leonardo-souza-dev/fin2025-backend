using Fin.Domain.Entities;
using Fin.Domain.Interfaces;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases;

public class RecurrenceService(IRecurrenceRepository repository, ITransactionRepository transactionRepository)
{
    public List<RecurrenceMessage> GetRecurrenceMessages()
    {
        var recurrences = repository.GetAll();
        var allTransactions = transactionRepository.GetAll();
        var recurrentTransactions = allTransactions.Where(t => 
            t.IsActive && t.RecurrenceId != null && 
            recurrences.Any(r => r.Id == t.RecurrenceId)
        );

        return recurrences.Select(r => new RecurrenceMessage
        {
            Message = $""
        }).ToList();
    }
}
