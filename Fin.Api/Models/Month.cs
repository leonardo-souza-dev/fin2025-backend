using Fin.Api.Services;

namespace Fin.Api.Models;

public class Month
{
    public decimal FinalBalancePreviousMonth { get; set; }
    public List<DayTransactions> DayTransactions  { get; set; } = [];

    public Month(int year, int month, string selectedAccountIds, IEnumerable<Account> allAccountsDb, IEnumerable<Transaction> allTransactionsDb)
    {
        var finalBalancePreviousMonth = new FinalBalancePreviousMonth(year, month, selectedAccountIds, allAccountsDb, allTransactionsDb);
        FinalBalancePreviousMonth = finalBalancePreviousMonth.Value;
        
        var transactionsOfTheSelectedAccounts = allTransactionsDb
            .Where(t => selectedAccountIds.Contains(t.FromAccountId.ToString()));

        var transactionsOfThisMonthAndSelectedAccounts = transactionsOfTheSelectedAccounts
            .Where(t => t.Date.Year == year && t.Date.Month == month).ToList();

        var dates = transactionsOfThisMonthAndSelectedAccounts
            .Select(transaction => transaction.Date)
            .Distinct()
            .OrderBy(date => date)
            .ToList();

        decimal initialBalance = FinalBalancePreviousMonth;
        for (int i = 0; i < dates.Count; i++)
        {
            DateOnly date = dates[i];
            var transactionsDate = transactionsOfThisMonthAndSelectedAccounts
                .Where(transaction => transaction.Date == date)
                .ToList();

            var dayTransaction = new DayTransactions(initialBalance, date, transactionsDate);
            DayTransactions.Add(dayTransaction);
            initialBalance = dayTransaction.FinalBalance;
        }
    }
}