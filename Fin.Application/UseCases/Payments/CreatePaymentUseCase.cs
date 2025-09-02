using System.Text.Json.Serialization;
using Fin.Domain.Entities;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases.Payments
{
    public sealed class CreatePaymentUseCase(IPaymentRepository repository, IUnitOfWork unitOfWork)
    {
        public CreatePaymentResponse Handle(CreatePaymentRequest request)
        {
            var payment = new Payment
            {
                Date = request.Date,
                Description = request.Description,
                FromAccountId = request.FromAccountId,
                Amount = request.Amount
            };
            repository.Create(payment);
            
            unitOfWork.SaveChanges();

            return new CreatePaymentResponse(payment);
        }
    }

    public sealed class CreatePaymentRequest
    {
        public required DateOnly Date { get; set; }
        public required string Description { get; set; }
        public required int FromAccountId { get; set; }
        public required decimal Amount { get; set; }
    }

    public sealed class CreatePaymentResponse
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string Description { get; set; }
        public int FromAccountId { get; set; }
        public decimal Amount { get; set; }
        
        [JsonConstructor]
        public CreatePaymentResponse(int id, DateOnly date, string description, int fromAccountId, decimal amount)
        {
            Id = id;
            Date = date;
            Description = description;
            FromAccountId = fromAccountId;
            Amount = amount;
        }

        public CreatePaymentResponse(Payment entity)
        {
            Id = entity.Id;
            Date = entity.Date;
            Description = entity.Description;
            FromAccountId = entity.FromAccountId;
            Amount = entity.Amount;
        }
    }
}