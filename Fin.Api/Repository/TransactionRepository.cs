using Fin.Api.Data;
using Fin.Api.Models;

namespace Fin.Api.Repository;

public class TransactionRepository(FinDbContext context) : ITransactionRepository
{
    public List<Transaction> GetAll(int? year = null, int? month = null)
    {
        if (year != null && month != null)
        {
            return [.. context.Transactions.Where(t => t.Date.Year == year && t.Date.Month == month && t.IsActive)];
        }

        return [.. context.Transactions.Where(t => t.IsActive)];
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

public interface ITransactionRepository
{
    List<Transaction> GetAll(int? year = null, int? month = null);
    void Create(Transaction transaction);
    void Update(Transaction transaction);
    void Delete(Transaction transaction);
}
