using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases
{
    public class UpdateTransferUseCase(
        IPaymentRepository paymentRepository,
        ITransferRepository transferRepository,
        IUnitOfWork unitOfWork)
    {
        public void Handle(UpdateTransferRequest request)
        {
            var transfer = transferRepository.Get(request.Id);
            if (transfer == null)
            {
                throw new InvalidOperationException("Transfer not found");
            }
        
            var paymentFrom = paymentRepository.Get(transfer.PaymentFromId);
            if (paymentFrom == null)
            {
                throw new InvalidOperationException($"Transfer id {transfer.PaymentFromId} not found");
            }
            var paymentTo = paymentRepository.Get(transfer.PaymentToId);
            if (paymentTo == null)
            {
                throw new InvalidOperationException($"Transfer id {transfer.PaymentToId} not found");
            }

            if (request.PaymentId == paymentFrom.Id)
            {
                paymentFrom.Date = request.Date;
                paymentFrom.Description = request.Description;
                paymentFrom.Amount = request.Amount;
                paymentFrom.FromAccountId = request.FromAccountId;
                paymentFrom.ToAccountId = request.ToAccountId;
                //TODO: recurrences
                
                paymentTo.Date = request.Date;
                paymentTo.Description = request.Description;
                paymentTo.Amount = request.Amount * -1;
                paymentTo.ToAccountId = request.FromAccountId;
                paymentTo.FromAccountId = request.ToAccountId;
            } else if (request.PaymentId == paymentTo.Id)
            {
                paymentTo.Date = request.Date;
                paymentTo.Description = request.Description;
                paymentTo.Amount = request.Amount;
                paymentTo.FromAccountId = request.FromAccountId;
                
                paymentFrom.Date = request.Date;
                paymentFrom.Description = request.Description;
                paymentFrom.Amount = request.Amount * -1;
                paymentFrom.ToAccountId = request.ToAccountId;
            }
        
            unitOfWork.SaveChanges();
        }
    }

    public class UpdateTransferRequest
    {
        public required int Id { get; set; }
        public required int PaymentId { get; set; }
        public required DateOnly Date { get; set; }
        public required string Description { get; set; }
        public required decimal Amount { get; set; }
        public required int FromAccountId { get; set; }
        public required int ToAccountId { get; set; }
        public required bool IsRecurrence { get; set; }
    }
}