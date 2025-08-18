using Fin.Domain.Entities;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Interfaces;

namespace Fin.Infrastructure.Repositories;

public class TransferRepository(FinDbContext context) : ITransferRepository
{
    public async Task Create(Transfer transfer)
    {
        ArgumentNullException.ThrowIfNull(transfer, nameof(transfer));
        transfer.IsActive = true;
        
        await context.Transfers.AddAsync(transfer);
    }
}
