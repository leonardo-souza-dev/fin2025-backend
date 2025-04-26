using Fin.Api.Models;
using Fin.Api.Repository;

namespace Fin.Api.Services;

public class MonthService(
    ITransactionRepository transactionRepository,
    IAccountRepository accountRepository)
{
    public Month Get(int yearNumber, int monthNumber, string selectedAccountIds)
    {
        if (string.IsNullOrEmpty(selectedAccountIds) || selectedAccountIds.Length == 0)
        {
            throw new ArgumentNullException("accountIds must have a value.");
        }
        if (monthNumber < 1 || monthNumber > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(monthNumber), "Month must be between 1 and 12.");
        }

        var month = new Month(yearNumber, monthNumber, selectedAccountIds, accountRepository.GetAll(), transactionRepository.GetAll());

        return month;
    }
}
