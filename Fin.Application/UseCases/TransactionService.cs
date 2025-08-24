using Fin.Domain.Entities;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases
{
    public class TransactionService(ITransactionRepository repository, ITransferRepository transferRepository2)
    {
        public Transaction UpdateSimple(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null");
            }

            repository.Update(transaction);

            return transaction;
        }

        public void Delete(int id)
        {
            var allTransactions = repository.GetAll();
            var transaction = allTransactions.FirstOrDefault(t => t.Id == id) ??
                              throw new ArgumentException($"Transaction with ID {id} not found", nameof(id));
            if (transaction.GetTransactionType() == "SIMPLE")
            {
                repository.Delete(transaction);
            }
            else
            {
                var allTransfers = transferRepository2.GetAll();
                var transfer = allTransfers.FirstOrDefault(t =>
                                   t.FromTransactionId == transaction.Id || t.ToTransactionId == transaction.Id)
                               ?? throw new ArgumentException(
                                   $"Transfer with Transaction ID {transaction.Id} not found", nameof(transaction.Id));

                transferRepository2.Delete(transfer);
            }
        }
    }
}