using Fin.Api.Models;
using Fin.Api.Repository;

namespace Fin.Api.Services;

public class TransactionService(
    ITransactionRepository repository,
    ITransferRepository transferRepository)
{
    public List<Transaction> GetAll(string monthSlashYear)
    {
        if (string.IsNullOrEmpty(monthSlashYear))
        {
            throw new ArgumentException("Month and year cannot be null or empty", nameof(monthSlashYear));
        }
        var monthYearParts = monthSlashYear.Split('/');
        if (monthYearParts.Length != 2 || !int.TryParse(monthYearParts[0], out var month) || !int.TryParse(monthYearParts[1], out var year))
        {
            throw new ArgumentException("Invalid month/year format. Expected format: MM/YYYY", nameof(monthSlashYear));
        }

        var transactions = repository.GetAll(year, month);
        
        return transactions;
    }

    public Transaction Create(Transaction transaction)
    {
        repository.Create(transaction);

        return transaction;
    }

    public dynamic CreateTransfer(Transaction fromTransaction)
    {
        repository.Create(fromTransaction);

        var toTransaction = fromTransaction.CreateRelatedTransfer();
        repository.Create(toTransaction);

        transferRepository.Create(fromTransaction.Id.Value, toTransaction.Id.Value);

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

    public Transaction Update(Transaction transaction)
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
        var transaction = repository.GetAll().FirstOrDefault(t => t.Id == id) ?? throw new ArgumentException($"Transaction with ID {id} not found", nameof(id));
        repository.Delete(transaction);
    }
}
