using Fin.Domain.Entities;
using Fin.Infrastructure.Data;

namespace Fin.Infrastructure.Repositories
{
    public interface IPaymentRepository
    {
        IQueryable<Payment> GetAll();
        Payment? Get(int id);
        Payment Create(Payment payment);
        void Update(Payment payment);
        void Delete(Payment payment);
    }

    public class PaymentRepository(FinDbContext context) : IPaymentRepository
    {
        public IEnumerable<Payment> GetAll(int? year = null, int? month = null)
        {
            var activePayments = context.Payments.Where(t => t.IsActive);

            if (year != null && month != null)
            {
                return [.. activePayments.Where(t => t.Date.Year == year && t.Date.Month == month)];
            }

            return [.. activePayments];
        }
        
        public IQueryable<Payment> GetAll()
        {
            var entities = context.Payments
                .Where(t => t.IsActive);
            return entities;
        }
        
        public Payment? Get(int id)
        {
            var payment = context.Payments.Find(id);
            return payment != null && payment.IsActive ? payment : null;;
        }

        public Payment Create(Payment payment)
        {
            ArgumentNullException.ThrowIfNull(payment, nameof(payment));

            payment.IsActive = true;

            context.Payments.Add(payment);

            return payment;
        }

        public void Update(Payment payment)
        {
            ArgumentNullException.ThrowIfNull(payment, nameof(payment));
            
            payment.IsActive = true;
            context.Payments.Update(payment);
        }

        public void Delete(Payment payment)
        {
            payment.IsActive = false;
        }
    }
}