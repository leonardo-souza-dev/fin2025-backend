using Fin.Api.Data;
using Fin.Api.Models;

namespace Fin.Api.Repository;

public class TransferRepository(FinDbContext context) : ITransferRepository
{
    public List<Transfer> GetAll()
    {
        return [.. context.Transfers];
    }

    public void Create(int fromTransactionId, int toTransactionId)
    {
        var transfer = new Transfer
        {
            FromTransactionId = fromTransactionId,
            ToTransactionId = toTransactionId
        };
        context.Transfers.Add(transfer);
        context.SaveChanges();
    }
}

public interface ITransferRepository
{
    List<Transfer> GetAll();
    void Create(int fromTransactionId, int toTransactionId);
}
