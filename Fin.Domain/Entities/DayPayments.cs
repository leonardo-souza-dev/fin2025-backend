namespace Fin.Domain.Entities;

public class DayPayments(
    decimal initialBalance,
    DateOnly day, 
    List<Payment> payments,
    IEnumerable<Transfer> allTransfersDb)
{
    public DateOnly Day { get; set; } = day;

    public List<Payment> Payments
    {
        get
        {
            List<Payment> newPayments = [];
            foreach (var payment in payments)
            {
                var transferRelated = allTransfersDb.FirstOrDefault(x => x.PaymentFromId == payment.Id || x.PaymentToId == payment.Id);
                
                if (transferRelated != null)
                {
                    payment.PaymentIdTransferRelated = transferRelated.PaymentFromId == payment.Id ? transferRelated.PaymentToId : transferRelated.PaymentFromId;
                    payment.TransferId = transferRelated.Id;
                }
                newPayments.Add(payment);
            }
            return newPayments;
        }
    }
    
    public decimal FinalBalance => payments.Sum(p => p.Amount) + initialBalance;
}