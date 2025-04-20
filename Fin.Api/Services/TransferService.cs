using Fin.Api.Data;
using Fin.Api.Models;

namespace Fin.Api.Services;

public class TransferService(FinDbContext context)
{
    private readonly FinDbContext _context = context;

    public Transaction Upsert(Transaction transactionRequest)
    {
        var isCreate = !transactionRequest.Id.HasValue;

        if (isCreate)
        {
            var transferCreating = new Transfer
            {
                Date = transactionRequest.Date,
                Description = transactionRequest.Description,
                SourceAccountId = transactionRequest.RefAccountId,
                Amount = transactionRequest.Amount,
                IsRecurrent = transactionRequest.IsRecurrent,
                DestinationAccountId = transactionRequest.OtherAccountId.Value,
                IsActive = transactionRequest.IsActive
            };
            _context.Transfers.Add(transferCreating);
            _context.SaveChanges();

            return new Transaction
            {
                Id = transferCreating.Id,
                Date = transactionRequest.Date,
                Description = transactionRequest.Description,
                RefAccountId = transactionRequest.RefAccountId,
                Amount = transactionRequest.Amount,
                Type = "TRANSFER",
                OtherAccountId = transactionRequest.OtherAccountId,
                IsRecurrent = transactionRequest.IsRecurrent,
                IsActive = transactionRequest.IsActive,
            };
        }
        else
        {
            var transferUpdating = _context.Transfers.FirstOrDefault(t => t.Id == transactionRequest.Id);

            if (transferUpdating != null)
            {
                transferUpdating.Date = transactionRequest.Date;
                transferUpdating.Description = transactionRequest.Description;
                transferUpdating.SourceAccountId = transactionRequest.RefAccountId;
                transferUpdating.Amount = transactionRequest.Amount;
                transferUpdating.IsRecurrent = transactionRequest.IsRecurrent;
                transferUpdating.DestinationAccountId = transactionRequest.OtherAccountId.Value;
                transferUpdating.IsActive = transactionRequest.IsActive;
                _context.Transfers.Update(transferUpdating);

                _context.SaveChanges();
            }
            return new Transaction
            {
                Id = transferUpdating.Id,
                Date = transactionRequest.Date,
                Description = transactionRequest.Description,
                RefAccountId = transactionRequest.RefAccountId,
                Amount = transactionRequest.Amount,
                Type = "TRANSFER",
                OtherAccountId = transactionRequest.OtherAccountId,
                IsRecurrent = transactionRequest.IsRecurrent,
                IsActive = transactionRequest.IsActive
            };
        }

    }
}
