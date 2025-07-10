using Fin.Api.DTO;
using Fin.Domain.Entities;
using Fin.Domain.Interfaces;

namespace Fin.Api.Services;

public class TransactionService(ITransactionRepository repository, ITransferRepository transferRepository)
{
    public Transaction CreateSimple(TransactionRequest transactionRequest)
    {
        if (transactionRequest.RecurrenceId.HasValue && transactionRequest.RecurrenceEndMonth.HasValue && transactionRequest.RecurrenceEndYear.HasValue)
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







    public dynamic CreateTransfer(Transaction fromTransaction)
    {
        repository.Create(fromTransaction);

        var toTransaction = fromTransaction.CreateRelatedTransfer();
        repository.Create(toTransaction);

        transferRepository.Create(new Transfer 
        { 
            FromTransactionId = fromTransaction.Id.Value, 
            ToTransactionId = toTransaction.Id.Value,
            IsActive = true
        });

        return new { fromTransaction, toTransaction };
    }

    public dynamic UpdateTransfer(Transaction transaction)
    {
        var transfer = transferRepository.GetAll()
            .FirstOrDefault(t => t.FromTransactionId == transaction.Id || t.ToTransactionId == transaction.Id) 
            ?? throw new ArgumentException($"Transfer with Transaction ID {transaction.Id} not found", nameof(transaction.Id));

        var fromTransaction = repository.GetAll().FirstOrDefault(t => t.Id == transfer.FromTransactionId);
        var toTransaction = repository.GetAll().FirstOrDefault(t => t.Id == transfer.ToTransactionId);

        if (fromTransaction == null || toTransaction == null)
        {
            throw new ArgumentException($"Transfer transactions not found.");
        }

        if (fromTransaction.Id == transaction.Id)
        {
            fromTransaction.Date = transaction.Date;
            fromTransaction.Description = transaction.Description;
            fromTransaction.FromAccountId = transaction.FromAccountId;
            fromTransaction.Amount = transaction.Amount;
            fromTransaction.ToAccountId = transaction.ToAccountId;
            fromTransaction.RecurrenceId = transaction.RecurrenceId;
            fromTransaction.IsActive = true;
            repository.Update(fromTransaction);

            toTransaction.Date = transaction.Date;
            toTransaction.Description = transaction.Description;
            toTransaction.FromAccountId = transaction.ToAccountId.Value;
            toTransaction.Amount = transaction.Amount * -1;
            toTransaction.ToAccountId = transaction.FromAccountId;
            toTransaction.RecurrenceId = transaction.RecurrenceId;
            toTransaction.IsActive = true;
            repository.Update(toTransaction);
        }
        else if (toTransaction.Id == transaction.Id)
        {

            toTransaction.Date = transaction.Date;
            toTransaction.Description = transaction.Description;
            toTransaction.FromAccountId = transaction.FromAccountId;
            toTransaction.Amount = transaction.Amount;
            toTransaction.ToAccountId = transaction.ToAccountId.Value;
            toTransaction.RecurrenceId = transaction.RecurrenceId;
            toTransaction.IsActive = true;
            repository.Update(toTransaction);

            fromTransaction.Date = transaction.Date;
            fromTransaction.Description = transaction.Description;
            fromTransaction.FromAccountId = transaction.ToAccountId.Value;
            fromTransaction.Amount = transaction.Amount * -1;
            fromTransaction.ToAccountId = transaction.FromAccountId;
            fromTransaction.RecurrenceId = transaction.RecurrenceId;
            fromTransaction.IsActive = true;
            repository.Update(fromTransaction);
        }

        return new { fromTransaction, toTransaction };
    }

    public void Delete(int id)
    {
        var transaction = repository.GetAll().FirstOrDefault(t => t.Id == id) ?? throw new ArgumentException($"Transaction with ID {id} not found", nameof(id));
        if (transaction.Type == "SIMPLE")
        {
            repository.Delete(transaction);
        }
        else
        {
            var transfer = transferRepository.GetAll()
                .FirstOrDefault(t => t.FromTransactionId == transaction.Id || t.ToTransactionId == transaction.Id)
                ?? throw new ArgumentException($"Transfer with Transaction ID {transaction.Id} not found", nameof(transaction.Id));

            transferRepository.Delete(transfer);
        }
        
    }
}
