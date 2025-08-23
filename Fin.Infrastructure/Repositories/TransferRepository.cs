using Fin.Domain.Entities;
using Fin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fin.Infrastructure.Repositories
{
    public interface ITransferRepository
    {
        IQueryable<Transfer> GetAll();
        Transfer? Get(int id);
        Transfer Create(Transfer transfer);
        void Update(Transfer transfer);
        void Delete(Transfer transfer);
    }
    
    public class TransferRepository : ITransferRepository
    {
        private readonly FinDbContext _context;

        public TransferRepository(FinDbContext context)
        {
            _context = context;
        }

        public IQueryable<Transfer> GetAll()
        {
            var entities = _context.Transfers.Where(a => a.IsActive);
            return entities;
        }

        public Transfer? Get(int id)
        {
            var transfer = _context.Transfers.Find(id);
            return transfer != null && transfer.IsActive ? transfer : null;;
        }

        public Transfer Create(Transfer transfer)
        {
            ArgumentNullException.ThrowIfNull(transfer, nameof(transfer));

            transfer.IsActive = true;

            _context.Transfers.Add(transfer);

            return transfer;
        }

        public void Update(Transfer transfer)
        {
            ArgumentNullException.ThrowIfNull(transfer, nameof(transfer));

            transfer.IsActive = true;
            _context.Transfers.Update(transfer);
        }

        public void Delete(Transfer transfer)
        {
            var fromTransaction = _context.Transactions.FirstOrDefault(t => t.Id == transfer.FromTransactionId) ??
                                  throw new ArgumentException($"Transaction with ID {transfer.FromTransactionId} not found", nameof(transfer.FromTransactionId));

            if (fromTransaction != null)
            {
                fromTransaction.IsActive = false;
            }

            var toTransaction = _context.Transactions.FirstOrDefault(t => t.Id == transfer.ToTransactionId) ??
                                throw new ArgumentException($"Transaction with ID {transfer.ToTransactionId} not found", nameof(transfer.ToTransactionId));

            if (toTransaction != null)
            {
                toTransaction.IsActive = false;
            }

            transfer.IsActive = false;
        }
    }
}