namespace Fin.Domain.Entities;

public class Month
{
    public decimal FinalBalancePreviousMonth { get; set; }
    public List<DayPayments> DayPayments  { get; set; } = [];

    public Month(
        int year, 
        int month, 
        List<int> selectedAccountIds, 
        IEnumerable<Account> allAccountsDb, 
        IEnumerable<Payment> allPaymentsDb,
        IEnumerable<Transfer> allTransfersDb)
    {
        var finalBalancePreviousMonth = new FinalBalancePreviousMonth(year, month, selectedAccountIds, allAccountsDb, allPaymentsDb);
        FinalBalancePreviousMonth = finalBalancePreviousMonth.Value;
        
        var paymentsOfTheSelectedAccounts = allPaymentsDb
            .Where(t => selectedAccountIds.Contains(t.FromAccountId));

        var paymentsOfThisMonthAndSelectedAccounts = paymentsOfTheSelectedAccounts
            .Where(t => t.Date.Year == year && t.Date.Month == month).ToList();

        var dates = paymentsOfThisMonthAndSelectedAccounts
            .Select(p => p.Date)
            .Distinct()
            .OrderBy(date => date)
            .ToList();

        decimal initialBalance = FinalBalancePreviousMonth;
        for (int i = 0; i < dates.Count; i++)
        {
            DateOnly date = dates[i];
            var paymentsDate = paymentsOfThisMonthAndSelectedAccounts
                .Where(p => p.Date == date)
                .ToList();

            var dayPayment = new DayPayments(initialBalance, date, paymentsDate, allTransfersDb);
            DayPayments.Add(dayPayment);
            initialBalance = dayPayment.FinalBalance;
        }
    }
}