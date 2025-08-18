using Microsoft.EntityFrameworkCore.Storage;

namespace Fin.Infrastructure.Data;

public sealed class UnitOfWork(FinDbContext context) : IUnitOfWork
{
    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return context.Database.BeginTransactionAsync();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}