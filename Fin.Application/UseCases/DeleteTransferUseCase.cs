using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases;

public class DeleteTransferUseCase(
    IPaymentRepository paymentRepository,
    ITransferRepository transferRepository,
    IUnitOfWork unitOfWork)
{
    public void Handle(int id)
    {
        try
        {
            var transfer = transferRepository.Get(id);
            if (transfer == null)
            {
                throw new ArgumentException($"Transfer with id {id} not found.");
            }
            
            var paymentFromId = paymentRepository.Get(transfer.PaymentFromId);
            if (paymentFromId == null)
            {
                throw new ArgumentException($"Payment with id {transfer.PaymentFromId} (related to transfer {id}) not found.");
            }
            
            var paymentToId = paymentRepository.Get(transfer.PaymentToId);
            if (paymentToId == null)
            {
                throw new InvalidOperationException($"Payment with id {transfer.PaymentToId} (related to transfer {id}) not found.");
            }
            
            transferRepository.Delete(transfer);
            paymentRepository.Delete(paymentFromId);
            paymentRepository.Delete(paymentToId);
            
            unitOfWork.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
