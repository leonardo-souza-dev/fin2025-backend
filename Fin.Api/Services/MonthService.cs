using Fin.Domain.Entities;
using Fin.Domain.Interfaces;

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

        var accountIds = selectedAccountIds.Split(',').Select(id => int.Parse(id)).ToList();

        var month = new Month(yearNumber, monthNumber, accountIds, accountRepository.GetAll(), transactionRepository.GetAll());

        return month;
    }
}
