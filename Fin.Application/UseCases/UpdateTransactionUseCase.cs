using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases
{
    public class UpdateTransactionUseCase(
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        public void Handle(UpdateTransactionRequest request)
        {
            var transaction = transactionRepository.Get(request.Id);

            if (transaction == null)
            {
                throw new InvalidDataException("Transaction not found.");
            }

            transaction.Date = request.Date;
            transaction.Description = request.Description;
            transaction.FromAccountId = request.FromAccountId;
            transaction.Amount = request.Amount;

            unitOfWork.SaveChanges();
        }
    }

    public class UpdateTransactionRequest
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string Description { get; set; }
        public int FromAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}