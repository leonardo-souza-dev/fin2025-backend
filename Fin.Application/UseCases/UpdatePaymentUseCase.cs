using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases
{
    public class UpdatePaymentUseCase(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
    {
        public void Handle(UpdatePaymentRequest request)
        {
            var payment = paymentRepository.Get(request.Id);

            if (payment == null)
            {
                throw new InvalidDataException("Payment not found.");
            }

            payment.Date = request.Date;
            payment.Description = request.Description;
            payment.FromAccountId = request.FromAccountId;
            payment.Amount = request.Amount;

            unitOfWork.SaveChanges();
        }
    }

    public class UpdatePaymentRequest
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string Description { get; set; }
        public int FromAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}