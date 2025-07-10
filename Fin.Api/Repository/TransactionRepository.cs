using Fin.Api.Data;
using Fin.Domain.Interfaces;
using Fin.Domain.Entities;

namespace Fin.Api.Repository;

public class TransactionRepository(FinDbContext context) : ITransactionRepository
{
    public IEnumerable<Transaction> GetAll(int? year = null, int? month = null)
    {
        var activeTransactions = context.Transactions.Where(t => t.IsActive);

        if (year != null && month != null)
        {
            return [.. activeTransactions.Where(t => t.Date.Year == year && t.Date.Month == month)];
        }

        return [.. activeTransactions];
    }

    public void Create(Transaction transaction)
    {
        context.Transactions.Add(transaction);
        context.SaveChanges();
    }

    public void Delete(Transaction transaction)
    {
        transaction.IsActive = false;
        context.Transactions.Update(transaction);
        context.SaveChanges();
    }


    public void DeleteTransfer(Transaction transaction1, Transaction transaction2)
    {
        transaction1.IsActive = false;
        context.Transactions.Update(transaction1);
        
        transaction2.IsActive = false;
        context.Transactions.Update(transaction2);

        context.SaveChanges();
    }

    public void Update(Transaction transaction)
    {
        var existingTransaction = context.Transactions.Find(transaction.Id);
        if (existingTransaction != null)
        {
            context.Entry(existingTransaction).CurrentValues.SetValues(transaction);
            context.SaveChanges();
        }
    }
}
