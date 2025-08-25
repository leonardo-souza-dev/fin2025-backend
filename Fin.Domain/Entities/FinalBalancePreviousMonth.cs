using Fin.Shared;

namespace Fin.Domain.Entities;

public class FinalBalancePreviousMonth(
    int year, 
    int month, 
    List<int> accountIds,
    IEnumerable<Account> accountDb,
    IEnumerable<Payment> payments)
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

            var allPreviousPayments = payments
                .Where(t => DateHelper.IsObj1BeforeObj2(t.Date.Year, t.Date.Month, year, month));

            var allPreviousPaymentsAccount = allPreviousPayments
                .Where(t => accountIds.Contains(t.FromAccountId))
                .ToList();
            var finalBalance = allPreviousPaymentsAccount.Sum(t => t.Amount);

            return finalBalance;
        }
    }
}