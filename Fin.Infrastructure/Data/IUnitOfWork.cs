using Microsoft.EntityFrameworkCore.Storage;

namespace Fin.Infrastructure.Data;

public interface IUnitOfWork
{
    Task<IDbContextTransaction> BeginTransactionAsync(); 
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
