using Fin.Api.Data;
using Fin.Api.Models;
using System.Threading.Tasks;

namespace Fin.Api.Services;

public class SimpleTransactionService(FinDbContext context)
{
    private readonly FinDbContext _context = context;

    public List<SimpleTransaction> GetAllActive()
    {
        return _context.SimpleTransactions
            .Where(a => a.IsActive)
            .ToList();
    }

    public SimpleTransaction Upsert(SimpleTransaction transaction)
    {
        SimpleTransaction simpleTransactionCreating;

        var isCreate = transaction.Id == 0;

        if (isCreate)
        {
            simpleTransactionCreating = new SimpleTransaction
            {
                Id = transaction.Id,
                Date = transaction.Date,
                Description = transaction.Description,
                AccountId = transaction.AccountId,
                Amount = transaction.Amount,
                IsRecurrent = transaction.IsRecurrent,
                IsActive = transaction.IsActive
            };

            var idCreate = _context.SimpleTransactions.Add(simpleTransactionCreating);
        }
        else
        {
            simpleTransactionCreating = _context.SimpleTransactions
                .FirstOrDefault(t => t.Id == transaction.Id);

            if (simpleTransactionCreating != null)
            {
                simpleTransactionCreating.Date = transaction.Date;
                simpleTransactionCreating.Description = transaction.Description;
                simpleTransactionCreating.AccountId = transaction.AccountId;
                simpleTransactionCreating.Amount = transaction.Amount;
                simpleTransactionCreating.IsRecurrent = transaction.IsRecurrent;
                simpleTransactionCreating.IsActive = transaction.IsActive;

                _context.SimpleTransactions.Update(simpleTransactionCreating);
            }
        }
        _context.SaveChanges();

        return simpleTransactionCreating;
    }

    public void Delete(int id)
    {
        var simpleTransaction = _context.SimpleTransactions
            .FirstOrDefault(t => t.Id == id);
        if (simpleTransaction != null)
        {
            simpleTransaction.IsActive = false;
            _context.SimpleTransactions.Update(simpleTransaction);
            _context.SaveChanges();
        }
    }
}
