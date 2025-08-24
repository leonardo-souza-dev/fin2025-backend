using System.Text.Json.Serialization;
using Fin.Domain.Entities;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases
{
    public class GetMonthUseCase(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository,
        ITransferRepository transferRepository
        ) 
    {
        public GetMonthResponse Handle(int yearNumber, int monthNumber, string selectedAccountIds)
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

            var allAccounts = accountRepository.GetAll();
            var allTransactions = transactionRepository.GetAll();
            var allTransfers = transferRepository.GetAll();
        
            var month = new Month(yearNumber, monthNumber, accountIds, allAccounts, allTransactions, allTransfers);

            var response = new GetMonthResponse(month);
            return response;
        }
    }

    public class GetMonthResponse
    {
        public decimal FinalBalancePreviousMonth { get; set; }
        public List<DayTransactionsDto> DayTransactions { get; set; } = [];

        public GetMonthResponse(Month month)
        {
            FinalBalancePreviousMonth = month.FinalBalancePreviousMonth;
            month.DayTransactions.ForEach(dt => DayTransactions.Add(new DayTransactionsDto(dt)));
        }
        
        [JsonConstructor]
        public GetMonthResponse(decimal finalBalancePreviousMonth, List<DayTransactionsDto> dayTransactions)
        {
            FinalBalancePreviousMonth = finalBalancePreviousMonth;
            DayTransactions = dayTransactions;
        }
    }

    public class DayTransactionsDto
    {
        public DateOnly Day { get; set; }
        public List<TransactionDto> Transactions { get; set; } = [];
        public decimal FinalBalance { get; set; }

        public DayTransactionsDto(DayTransactions dayTransactions)
        {
            Day = dayTransactions.Day;
            dayTransactions.Transactions.ForEach(t => Transactions.Add(new TransactionDto(t)));
            FinalBalance = dayTransactions.FinalBalance;
        }

        [JsonConstructor]
        public DayTransactionsDto(DateOnly day, List<TransactionDto> transactions, decimal finalBalance)
        {
            Day = day;
            Transactions = transactions;
            FinalBalance = finalBalance;
        }
    }

    public class TransactionDto
    {
        public int Id { get; init; }
        public DateOnly Date { get; set; }
        public string Description { get; set; }
        public int FromAccountId { get; set; }
        public decimal Amount { get; set; }
        public int? ToAccountId { get; set; }
        public int? TransactionIdTransferRelated { get; set; }
        public int? TransferId { get; set; }
        public int? RecurrenceId { get; set; }

        public TransactionDto(Transaction transaction)
        {
            Id = transaction.Id;
            Date = transaction.Date;
            Description = transaction.Description;
            FromAccountId = transaction.FromAccountId;
            Amount = transaction.Amount;
            ToAccountId = transaction.ToAccountId;
            TransactionIdTransferRelated = transaction.TransactionIdTransferRelated;
            TransferId = transaction.TransferId;
            RecurrenceId = transaction.RecurrenceId;
        }

        [JsonConstructor]
        public TransactionDto(int id, DateOnly date, string description, int fromAccountId, decimal amount, int? toAccountId, int? transactionIdTransferRelated, int? transferId, int? recurrenceId)
        {
            Id = id;
            Date = date;
            Description = description;
            FromAccountId = fromAccountId;
            Amount = amount;
            ToAccountId = toAccountId;
            TransactionIdTransferRelated = transactionIdTransferRelated;
            TransferId = transferId;
            RecurrenceId = recurrenceId;
        }
        
    }
}