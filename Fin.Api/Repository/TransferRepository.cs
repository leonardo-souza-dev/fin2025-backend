using Fin.Api.Data;
using Fin.Domain.Entities;
using Fin.Domain.Interfaces;

namespace Fin.Api.Repository;

public class TransferRepository(FinDbContext context) : ITransferRepository
{
    public IEnumerable<Transfer> GetAll()
    {
        return [.. context.Transfers.Where(t => t.IsActive)];
    }

    public void Create(Transfer transfer)
    {
        context.Transfers.Add(transfer);
        context.SaveChanges();
    }

    public void Delete(Transfer transfer)
    {
        var fromTransaction = context.Transactions.FirstOrDefault(t => t.Id == transfer.FromTransactionId) ??
            throw new ArgumentException($"Transaction with ID {transfer.FromTransactionId} not found", nameof(transfer.FromTransactionId));

        if (fromTransaction != null)
        {
            fromTransaction.IsActive = false;
        }

        var toTransaction = context.Transactions.FirstOrDefault(t => t.Id == transfer.ToTransactionId) ??
            throw new ArgumentException($"Transaction with ID {transfer.ToTransactionId} not found", nameof(transfer.ToTransactionId));

        if (toTransaction != null)
        {
            toTransaction.IsActive = false;
        }

        transfer.IsActive = false;
        context.SaveChanges();
    }
}


