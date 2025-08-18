using Fin.Domain.Entities;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Interfaces;

namespace Fin.Infrastructure.Repositories;

public class TransactionRepository(FinDbContext context) : ITransactionRepository
{
    public async Task Create(Transaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction, nameof(transaction));
        
        await context.Transactions.AddAsync(transaction);
    }
}