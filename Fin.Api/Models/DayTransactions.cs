namespace Fin.Api.Models;

public class DayTransactions(
    decimal initialBalance,
    DateOnly day, 
    List<Transaction> transactions)
{
    public DateOnly Day { get; set; } = day;
    public List<Transaction> Transactions { get; set; } = transactions;
    public decimal FinalBalance => transactions.Sum(transaction => transaction.Amount) + initialBalance;
}