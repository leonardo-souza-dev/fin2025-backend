using Fin.Domain.Entities;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases.Transfers
{
    public class CreateTransferUseCase(
        IPaymentRepository paymentRepository, 
        ITransferRepository transferRepository,
        IUnitOfWork unitOfWork)
    {
        public CreateTransferResponse Handle(CreateTransferRequest request, CancellationToken cancellationToken = default)
        {
            var paymentFrom = new Payment
            {
                Date = request.Date,
                Description = request.Description,
                FromAccountId = request.FromAccountId,
                Amount = request.Amount,
                ToAccountId = request.ToAccountId,
            };
            var paymentTo = new Payment
            {
                Date = request.Date,
                Description = request.Description,
                FromAccountId = request.ToAccountId,
                Amount = request.Amount * -1,
                ToAccountId = request.FromAccountId,
            };

            using var uowTransaction = unitOfWork.BeginTransaction();

            try
            {
                var paymentFromCreated = paymentRepository.Create(paymentFrom);
                var paymentToCreated = paymentRepository.Create(paymentTo);
                unitOfWork.SaveChanges();

                var transfer = new Transfer
                {
                    PaymentFromId = paymentFromCreated.Id,
                    PaymentToId = paymentToCreated.Id
                };
                _ = transferRepository.Create(transfer);
                unitOfWork.SaveChanges();

                uowTransaction.Commit();
                
                return CreateTransferResponse.Of(transfer);
            }
            catch (Exception ex)
            {
                uowTransaction.Rollback();
                Console.WriteLine(ex);
                throw;
            }
        }
    }

    public class CreateTransferRequest
    {
        public required DateOnly Date { get; set; }
        public required string Description { get; set; }
        public required int FromAccountId { get; set; }
        public required decimal Amount { get; set; }
        public required int ToAccountId { get; set; }
        public required bool IsRecurrence { get; set; }
    }

    public class CreateTransferResponse
    {
        public int Id { get; set; }
        public int PaymentFromId { get; set; }
        public int PaymentToId { get; set; }

        public static CreateTransferResponse Of(Transfer transfer)
        {
            return new CreateTransferResponse
            {
                Id = transfer.Id,
                PaymentFromId = transfer.PaymentFromId,
                PaymentToId = transfer.PaymentToId
            };
        }
    }
}