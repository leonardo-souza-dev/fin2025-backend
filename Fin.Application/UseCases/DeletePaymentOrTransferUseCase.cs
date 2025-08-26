using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases;

public class DeletePaymentOrTransferUseCase(
    IPaymentRepository paymentRepository,
    ITransferRepository transferRepository,
    IUnitOfWork unitOfWork)
{
    public void Handle(int paymentId)
    {
        try
        {
            var payment = paymentRepository.Get(paymentId);
            if (payment == null)
            {
                throw new ArgumentException($"Payment with id {paymentId} not found.");
            }
            paymentRepository.Delete(payment);
            
            var transferRelated = transferRepository.GetAll()
                .FirstOrDefault(x => x.PaymentFromId == payment.Id || x.PaymentToId == payment.Id);
            if (transferRelated != null)
            {
                var paymentRelatedId = transferRelated.PaymentFromId == payment.Id ? transferRelated.PaymentToId : transferRelated.PaymentFromId;
                var paymentRelated = paymentRepository.Get(paymentRelatedId);
                if (paymentRelated == null)
                {
                    throw new ArgumentException($"Payment with id {payment.Id} not found.");
                }
                paymentRepository.Delete(paymentRelated);
                transferRepository.Delete(transferRelated);
            }
            unitOfWork.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public void HandleBkp(int paymentId)
    {
        try
        {
            var payment = paymentRepository.Get(paymentId);
            if (payment == null)
            {
                throw new ArgumentException($"Payment with id {paymentId} not found.");
            }
            paymentRepository.Delete(payment);
            
            var transferRelated = transferRepository.GetAll()
                .FirstOrDefault(x => x.PaymentFromId == payment.Id || x.PaymentToId == payment.Id);
            if (transferRelated != null)
            {
                var paymentRelatedId = transferRelated.PaymentFromId == payment.Id ? transferRelated.PaymentToId : transferRelated.PaymentFromId;
                var paymentRelated = paymentRepository.Get(paymentRelatedId);
                if (paymentRelated == null)
                {
                    throw new ArgumentException($"Payment with id {payment.Id} not found.");
                }
                paymentRepository.Delete(paymentRelated);
                transferRepository.Delete(transferRelated);
            }
            unitOfWork.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
