using Fin.Shared;

namespace Fin.Domain.Entities;

public class FinalBalancePreviousMonth(
    int year, 
    int month, 
    List<int> accountIds,
    IEnumerable<Account> accountDb,
    IEnumerable<Transaction> transactions)
{
    public decimal Value
    { 
        get
        {
            var accountsNotFound = new List<int>();

            foreach (var accountId in accountIds)
            {
                var account = accountDb.FirstOrDefault(a => a.Id == accountId);
                if (account == null)
                {
                    accountsNotFound.Add(accountId);
                }
            }

            if (accountsNotFound.Count > 0)
            {
                throw new ArgumentException($"Accounts not found: {string.Join(", ", accountsNotFound.Select(id => id))}", nameof(accountIds));
            }

            var allPreviousTransactions = transactions
                .Where(t => DateHelper.IsObj1BeforeObj2(t.Date.Year, t.Date.Month, year, month));

            var allPreviousTransactionsAccount = allPreviousTransactions
                .Where(t => accountIds.Contains(t.FromAccountId))
                .ToList();
            var finalBalance = allPreviousTransactionsAccount.Sum(t => t.Amount);

            return finalBalance;
        }
    }
}