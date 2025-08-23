namespace Fin.Domain.Entities;

public class DayTransactions(
    decimal initialBalance,
    DateOnly day, 
    List<Transaction> transactions,
    IEnumerable<Transfer> allTransfersDb)
{
    public DateOnly Day { get; set; } = day;

    public List<Transaction> Transactions
    {
        get
        {
            List<Transaction> newTransactions = [];
            foreach (var t in transactions)
            {
                var transferRelated = allTransfersDb.FirstOrDefault(x => x.FromTransactionId == t.Id || x.ToTransactionId == t.Id);
                
                if (transferRelated != null)
                {
                    t.TransactionIdTransferRelated = transferRelated.FromTransactionId == t.Id ? transferRelated.ToTransactionId : transferRelated.FromTransactionId;
                    t.TransferId = transferRelated.Id;
                }
                newTransactions.Add(t);
            }
            return newTransactions;
        }
    }
    
    public decimal FinalBalance => transactions.Sum(transaction => transaction.Amount) + initialBalance;
}