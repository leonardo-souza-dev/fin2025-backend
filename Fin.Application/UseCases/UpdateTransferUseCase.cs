using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases
{
    // public interface IUpdateTransferUseCase
    // {
    //     void Handle(UpdateTransferRequest request);
    // }
    
    public class UpdateTransferUseCase(
        ITransactionRepository transactionRepository,
        ITransferRepository transferRepository,
        IUnitOfWork unitOfWork)/* : IUpdateTransferUseCase*/
    {
        public void Handle(UpdateTransferRequest request)
        {
            var transfer = transferRepository.Get(request.Id);
            if (transfer == null)
            {
                throw new InvalidOperationException("Transfer not found");
            }
        
            var transactionFrom = transactionRepository.Get(transfer.FromTransactionId);
            if (transactionFrom == null)
            {
                throw new InvalidOperationException($"Transfer id {transfer.FromTransactionId} not found");
            }
            var transactionTo = transactionRepository.Get(transfer.ToTransactionId);
            if (transactionTo == null)
            {
                throw new InvalidOperationException($"Transfer id {transfer.ToTransactionId} not found");
            }

            if (request.TransactionId == transactionFrom.Id)
            {
                transactionFrom.Date = request.Date;
                transactionFrom.Description = request.Description;
                transactionFrom.Amount = request.Amount;
                transactionFrom.FromAccountId = request.FromAccountId;
                transactionFrom.ToAccountId = request.ToAccountId;
                //TODO: recurrences
                
                transactionTo.Date = request.Date;
                transactionTo.Description = request.Description;
                transactionTo.Amount = request.Amount * -1;
                transactionTo.ToAccountId = request.FromAccountId;
                transactionTo.FromAccountId = request.ToAccountId;
            } else if (request.TransactionId == transactionTo.Id)
            {
                transactionTo.Date = request.Date;
                transactionTo.Description = request.Description;
                transactionTo.Amount = request.Amount;
                transactionTo.FromAccountId = request.FromAccountId;
                
                transactionFrom.Date = request.Date;
                transactionFrom.Description = request.Description;
                transactionFrom.Amount = request.Amount * -1;
                transactionFrom.ToAccountId = request.ToAccountId;
            }
        
            unitOfWork.SaveChanges();
        }
    }

    public class UpdateTransferRequest
    {
        public required int Id { get; set; }
        public required int TransactionId { get; set; }
        public required DateOnly Date { get; set; }
        public required string Description { get; set; }
        public required decimal Amount { get; set; }
        public required int FromAccountId { get; set; }
        public required int ToAccountId { get; set; }
        public required bool IsRecurrence { get; set; }
    }
}