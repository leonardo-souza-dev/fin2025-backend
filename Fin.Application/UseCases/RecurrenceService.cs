using Fin.Domain.Entities;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases;

public class RecurrenceService(IRecurrenceRepository repository, IPaymentRepository paymentRepository)
{
    public List<RecurrenceMessage> GetRecurrenceMessages()
    {
        var recurrences = repository.GetAll();
        var allPayments = paymentRepository.GetAll();
        var recurrentPayments = allPayments.Where(t => 
            t.IsActive && t.RecurrenceId != null && 
            recurrences.Any(r => r.Id == t.RecurrenceId)
        );

        return recurrences.Select(r => new RecurrenceMessage
        {
            Message = $""
        }).ToList();
    }
}
