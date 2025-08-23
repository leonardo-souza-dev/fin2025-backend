using Fin.Domain.Entities;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases
{
    public sealed class TransactionRequest
    {
        public int? Id { get; set; }
        public required DateOnly Date { get; set; }
        public required string Description { get; set; }
        public required int FromAccountId { get; set; }
        public required decimal Amount { get; set; }
        public required int? ToAccountId { get; set; }
        public int? RecurrenceId { get; set; }

        public int? RecurrenceEndMonth { get; set; }
        public int? RecurrenceEndYear { get; set; }
    }

    public class TransactionService(
        ITransactionRepository repository,
        ITransferRepository transferRepository2)
    {
        public Transaction CreateSimple(TransactionRequest transactionRequest)
        {
            if (transactionRequest.RecurrenceId.HasValue && transactionRequest.RecurrenceEndMonth.HasValue &&
                transactionRequest.RecurrenceEndYear.HasValue)
            {
                var numberOcurrences =
                    (transactionRequest.RecurrenceEndYear.Value - transactionRequest.Date.Year) * 12 +
                    (transactionRequest.RecurrenceEndMonth.Value - transactionRequest.Date.Month) + 1;

                Transaction firstTransaction = null;
                for (var i = 0; i < numberOcurrences; i++)
                {
                    var date = transactionRequest.Date.AddMonths(i).AddDays(transactionRequest.Date.Day - 1);
                    var newTransaction = new Transaction
                    {
                        Date = new DateOnly(date.Year, date.Month, date.Day),
                        Description = transactionRequest.Description,
                        FromAccountId = transactionRequest.FromAccountId,
                        Amount = transactionRequest.Amount,
                        ToAccountId = transactionRequest.ToAccountId,
                        RecurrenceId = transactionRequest.RecurrenceId,
                        IsActive = true
                    };
                    repository.Create(newTransaction);
                    if (i == 0)
                    {
                        firstTransaction = newTransaction;
                    }
                }

                return firstTransaction;
            }

            var transaction = new Transaction
            {
                Date = transactionRequest.Date,
                Description = transactionRequest.Description,
                FromAccountId = transactionRequest.FromAccountId,
                Amount = transactionRequest.Amount,
                ToAccountId = transactionRequest.ToAccountId,
                RecurrenceId = transactionRequest.RecurrenceId,
                IsActive = true
            };
            repository.Create(transaction);

            return transaction;
        }

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