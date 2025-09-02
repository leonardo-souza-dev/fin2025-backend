using System.Text.Json.Serialization;
using Fin.Domain.Entities;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases.Months
{
    public class GetMonthUseCase(
        IPaymentRepository paymentRepository,
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
            var allPayments = paymentRepository.GetAll();
            var allTransfers = transferRepository.GetAll();
        
            var month = new Month(yearNumber, monthNumber, accountIds, allAccounts, allPayments, allTransfers);

            var response = new GetMonthResponse(month);
            return response;
        }
    }

    public class GetMonthResponse
    {
        public decimal FinalBalancePreviousMonth { get; set; }
        public List<DayPaymentsDto> DayPayments { get; set; } = [];

        public GetMonthResponse(Month month)
        {
            FinalBalancePreviousMonth = month.FinalBalancePreviousMonth;
            month.DayPayments.ForEach(dt => DayPayments.Add(new DayPaymentsDto(dt)));
        }
        
        [JsonConstructor]
        public GetMonthResponse(decimal finalBalancePreviousMonth, List<DayPaymentsDto> dayPayments)
        {
            FinalBalancePreviousMonth = finalBalancePreviousMonth;
            DayPayments = dayPayments;
        }
    }

    public class DayPaymentsDto
    {
        public DateOnly Day { get; set; }
        public List<PaymentDto> Payments { get; set; } = [];
        public decimal FinalBalance { get; set; }

        public DayPaymentsDto(DayPayments dayPayments)
        {
            Day = dayPayments.Day;
            dayPayments.Payments.ForEach(t => Payments.Add(new PaymentDto(t)));
            FinalBalance = dayPayments.FinalBalance;
        }

        [JsonConstructor]
        public DayPaymentsDto(DateOnly day, List<PaymentDto> payments, decimal finalBalance)
        {
            Day = day;
            Payments = payments;
            FinalBalance = finalBalance;
        }
    }

    public class PaymentDto
    {
        public int Id { get; init; }
        public DateOnly Date { get; set; }
        public string Description { get; set; }
        public int FromAccountId { get; set; }
        public decimal Amount { get; set; }
        public int? ToAccountId { get; set; }
        public int? PaymentIdTransferRelated { get; set; }
        public int? TransferId { get; set; }
        public int? RecurrenceId { get; set; }

        public PaymentDto(Payment payment)
        {
            Id = payment.Id;
            Date = payment.Date;
            Description = payment.Description;
            FromAccountId = payment.FromAccountId;
            Amount = payment.Amount;
            ToAccountId = payment.ToAccountId;
            PaymentIdTransferRelated = payment.PaymentIdTransferRelated;
            TransferId = payment.TransferId;
            RecurrenceId = payment.RecurrenceId;
        }

        [JsonConstructor]
        public PaymentDto(int id, DateOnly date, string description, int fromAccountId, decimal amount, int? toAccountId, int? paymentIdTransferRelated, int? transferId, int? recurrenceId)
        {
            Id = id;
            Date = date;
            Description = description;
            FromAccountId = fromAccountId;
            Amount = amount;
            ToAccountId = toAccountId;
            PaymentIdTransferRelated = paymentIdTransferRelated;
            TransferId = transferId;
            RecurrenceId = recurrenceId;
        }
        
    }
}