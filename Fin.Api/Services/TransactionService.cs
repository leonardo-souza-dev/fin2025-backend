using Fin.Api.Data;
using Fin.Api.Models;

namespace Fin.Api.Services;

public class TransactionService(
    SimpleTransactionService simpleTransactionService,
    RecurrenceService recurrenceService,
    FinDbContext context)
{
    private readonly SimpleTransactionService _simpleTransactionService = simpleTransactionService;
    private readonly RecurrenceService _recurrenceService = recurrenceService;
    private readonly FinDbContext _context = context;

    public List<Transaction> GetAllActive(string monthSlashYear)
    {
        List<Transaction> transactions = [];

        var simpleTransactionsActive = _simpleTransactionService.GetAllActive();
        simpleTransactionsActive.ForEach(simpleTransactionActive =>
        {
            transactions.Add(new Transaction
            {
                Id = simpleTransactionActive.Id,
                Date = simpleTransactionActive.Date,
                Description = simpleTransactionActive.Description,
                RefAccountId = simpleTransactionActive.AccountId,
                Amount = simpleTransactionActive.Amount,
                Type = "SIMPLE",
                IsRecurrent = simpleTransactionActive.IsRecurrent,
                IsActive = simpleTransactionActive.IsActive
            });
        });

        var activeTransfers = _context.Transfers.Where(a => a.IsActive).ToList();
        activeTransfers.ForEach(transfer =>
        {
            var transactionSource = new Transaction
            {
                Id = transfer.Id,
                Date = transfer.Date,
                Description = transfer.Description,
                RefAccountId = transfer.SourceAccountId,
                Amount = transfer.Amount,
                Type = "TRANSFER",
                OtherAccountId = transfer.DestinationAccountId,
                IsRecurrent = transfer.IsRecurrent,
                IsActive = transfer.IsActive
            };
            transactions.Add(transactionSource);

            var transactionDestination = new Transaction
            {
                Id = transfer.Id,
                Date = transfer.Date,
                Description = transfer.Description,
                RefAccountId = transfer.DestinationAccountId,
                Amount = transfer.Amount * -1,
                Type = "TRANSFER",
                OtherAccountId = transfer.SourceAccountId,
                IsRecurrent = transfer.IsRecurrent,
                IsActive = transfer.IsActive
            };
            transactions.Add(transactionDestination);
        });

        var monthSlashYearSplit = monthSlashYear.Split("/");
        var year = int.Parse(monthSlashYearSplit[1]);
        var month = int.Parse(monthSlashYearSplit[0]);

        var monthDashYear = $"{year}-{month:00}";

        var recurrences = _recurrenceService.GetAllActive(monthDashYear);
        foreach (var recurrence in recurrences) // Replaced ForEach with foreach loop
        {
            var date = new DateOnly(year, month, recurrence.Day);
            var transaction = new Transaction
            {
                Id = recurrence.Id,
                Date = date,
                Description = recurrence.Name,
                RefAccountId = recurrence.AccountId,
                Amount = recurrence.Amount,
                Type = "RECURRENCE",
                IsRecurrent = true,
                IsActive = recurrence.IsActive
            };
            transactions.Add(transaction);
        }

        return transactions;
    }

    public Transaction Upsert(Transaction transactionRequest)
    {
        if (transactionRequest == null)
        {
            throw new ArgumentNullException(nameof(transactionRequest), "Transaction cannot be null");
        }

        var isCreate = !transactionRequest.Id.HasValue;
        var type = transactionRequest.OtherAccountId == null ? "SIMPLE" : "TRANSFER";

        if (type == "SIMPLE")
        {
            var simpleTransactionUpserted = _simpleTransactionService.Upsert(new SimpleTransaction
            {
                Id = transactionRequest.Id,
                Date = transactionRequest.Date,
                Description = transactionRequest.Description,
                AccountId = transactionRequest.RefAccountId,
                Amount = transactionRequest.Amount,
                IsRecurrent = transactionRequest.IsRecurrent,
                IsActive = transactionRequest.IsActive
            });

            return new Transaction
            {
                Id = simpleTransactionUpserted.Id,
                Date = transactionRequest.Date,
                Description = transactionRequest.Description,
                RefAccountId = transactionRequest.RefAccountId,
                Amount = transactionRequest.Amount,
                Type = "SIMPLE",
                OtherAccountId = transactionRequest.OtherAccountId,
                IsRecurrent = transactionRequest.IsRecurrent,
                IsActive = transactionRequest.IsActive
            };
        }

        if (isCreate && type == "TRANSFER")
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
        } else if (!isCreate && type == "TRANSFER")
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

        throw new Exception("Error on upsert transaction.");
    }

    public void Delete(string idType)
    {
        if (string.IsNullOrEmpty(idType))
        {
            throw new ArgumentNullException(nameof(idType), "IdType cannot be null or empty");
        }

        var idTypeObj = TransactionService.GetIdType(idType);
        var idTypeObjType = (string)idTypeObj.Type;
        var idTypeObjId = (int)idTypeObj.Id;

        if (idTypeObjType == "SIMPLE")
        {
            _simpleTransactionService.Delete(idTypeObjId);
        }
        else if (idTypeObjType == "TRANSFER")
        {
            var transfer = _context.Transfers.Where(t => t.Id == idTypeObjId).FirstOrDefault();

            if (transfer != null)
            {
                transfer.IsActive = false;
                _context.Transfers.Update(transfer);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception($"Transfer with type ${idTypeObjType} not found.");
            }
        }
        else
        {
            throw new Exception($"Transfer with type ${idTypeObjType} not found.");
        }
    }

    private static dynamic GetIdType(string idType)
    {
        int id;
        var type = "";

        if (idType.IndexOf("_") > 0)
        {
            var idTypeSplit = idType.Split("_");
            id = int.Parse(idTypeSplit[0]);
            type = idTypeSplit[1];
        }
        else
        {
            id = int.Parse(idType);
        }

        return new { Id = id, Type = type };
    }
}
