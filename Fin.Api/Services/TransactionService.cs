using Fin.Api.Data;
using Fin.Api.Models;

namespace Fin.Api.Services;

public class TransactionService(
    SimpleTransactionService simpleTransactionService,
    RecurrenceService recurrenceService,
    TransferService transferService,
    FinDbContext context)
{
    private readonly SimpleTransactionService _simpleTransactionService = simpleTransactionService;
    private readonly RecurrenceService _recurrenceService = recurrenceService;
    private readonly TransferService _transferService = transferService;
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

        var type = transactionRequest.OtherAccountId == null ? "SIMPLE" : "TRANSFER";

        if (type == "SIMPLE")
        {
            return _simpleTransactionService.Upsert(transactionRequest);
        }

        if (type == "TRANSFER")
        {
            return _transferService.Upsert(transactionRequest);
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
