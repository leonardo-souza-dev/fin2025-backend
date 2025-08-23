using Fin.Domain.Entities;
using Fin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fin.Infrastructure.Repositories
{
    public interface ITransactionRepository
    {
        IQueryable<Transaction> GetAll();
        Transaction? Get(int id);
        Transaction Create(Transaction transaction);
        void Update(Transaction transaction);
        void Delete(Transaction transaction);
    }

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
        
        public IQueryable<Transaction> GetAll()
        {
            var entities = context.Transactions
                .Where(t => t.IsActive);
            return entities;
        }

        public Transaction? FindAsync(int id)
        {
            var transaction = context.Transactions.Find(id);
            return transaction != null && transaction.IsActive ? transaction : null;
        }
        
        public Transaction? Get(int id)
        {
            var transaction = context.Transactions.Find(id);
            return transaction != null && transaction.IsActive ? transaction : null;;
        }

        public Transaction Create(Transaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction, nameof(transaction));

            transaction.IsActive = true;

            context.Transactions.Add(transaction);

            return transaction;
        }

        public void Update(Transaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction, nameof(transaction));
            
            transaction.IsActive = true;
            context.Transactions.Update(transaction);
        }

        public void Delete(Transaction transaction)
        {
            transaction.IsActive = false;
            context.Transactions.Update(transaction);
        }
    }
}