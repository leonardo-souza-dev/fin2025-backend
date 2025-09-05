using Fin.Domain.Entities;
using Fin.Infrastructure.Data;

namespace Fin.Infrastructure.Repositories
{
    public interface ITransferRepository
    {
        IQueryable<Transfer> GetAll();
        Transfer? Get(int id);
        Transfer Create(Transfer transfer);
        void Delete(Transfer transfer);
    }
    
    public class TransferRepository(FinDbContext context) : ITransferRepository
    {
        public IQueryable<Transfer> GetAll()
        {
            var entities = context.Transfers.Where(a => a.IsActive);
            return entities;
        }

        public Transfer? Get(int id)
        {
            var transfer = context.Transfers.Find(id);
            return transfer != null && transfer.IsActive ? transfer : null;
        }

        public Transfer Create(Transfer transfer)
        {
            ArgumentNullException.ThrowIfNull(transfer, nameof(transfer));

            transfer.IsActive = true;

            context.Transfers.Add(transfer);

            return transfer;
        }

        public void Delete(Transfer transfer)
        {
            transfer.IsActive = false;
        }
    }
}