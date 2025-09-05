using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases.Payments
{
    public class EditPaymentUseCase(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
    {
        public void Handle(EditPaymentRequest request)
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

    public class EditPaymentRequest
    {
        public required int Id { get; set; }
        public required DateOnly Date { get; set; }
        public required string Description { get; set; }
        public required int FromAccountId { get; set; }
        public required decimal Amount { get; set; }
    }
}