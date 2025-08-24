using Fin.Domain.Entities;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases
{
    public class CreateTransferUseCase(
        ITransactionRepository transactionRepository,
        ITransferRepository transferRepository,
        IUnitOfWork unitOfWork)
    {
        public void Handle(CreateTransferRequest request, CancellationToken cancellationToken = default)
        {
            var transactionFrom = new Transaction
            {
                Date = request.Date,
                Description = request.Description,
                FromAccountId = request.FromAccountId,
                Amount = request.Amount,
                ToAccountId = request.ToAccountId,
            };
            var transactionTo = new Transaction
            {
                Date = request.Date,
                Description = request.Description,
                FromAccountId = request.ToAccountId,
                Amount = request.Amount * -1,
                ToAccountId = request.FromAccountId,
            };

            using var uowTransaction = unitOfWork.BeginTransaction();

            try
            {
                var transactionFromCreated = transactionRepository.Create(transactionFrom);
                var transactionToCreated = transactionRepository.Create(transactionTo);

                var transfer = new Transfer
                {
                    FromTransactionId = transactionFromCreated.Id,
                    ToTransactionId = transactionToCreated.Id
                };
                _ = transferRepository.Create(transfer);
                unitOfWork.SaveChanges();

                uowTransaction.Commit();
            }
            catch (Exception ex)
            {
                uowTransaction.Rollback();
                Console.WriteLine(ex);
                throw;
            }
        }
    }

    public class CreateTransferRequest
    {
        public required DateOnly Date { get; set; }
        public required string Description { get; set; }
        public required int FromAccountId { get; set; }
        public required decimal Amount { get; set; }
        public required int ToAccountId { get; set; }
        public required bool IsRecurrence { get; set; }
    }
}