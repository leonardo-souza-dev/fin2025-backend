using System.Text.Json.Serialization;
using Fin.Domain.Entities;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases
{
    public sealed class CreateTransactionUseCase(
        ITransactionRepository repository,
        IUnitOfWork unitOfWork)
    {
        public CreateTransactionResponse Handle(CreateTransactionRequest request)
        {
            var transaction = new Transaction
            {
                Date = request.Date,
                Description = request.Description,
                FromAccountId = request.FromAccountId,
                Amount = request.Amount
            };
            repository.Create(transaction);
            
            unitOfWork.SaveChanges();

            return new CreateTransactionResponse(transaction);
        }
    }

    public sealed class CreateTransactionRequest
    {
        public required DateOnly Date { get; set; }
        public required string Description { get; set; }
        public required int FromAccountId { get; set; }
        public required decimal Amount { get; set; }
    }

    public sealed class CreateTransactionResponse
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string Description { get; set; }
        public int FromAccountId { get; set; }
        public decimal Amount { get; set; }
        
        [JsonConstructor]
        public CreateTransactionResponse(int id, DateOnly date, string description, int fromAccountId, decimal amount)
        {
            Id = id;
            Date = date;
            Description = description;
            FromAccountId = fromAccountId;
            Amount = amount;
        }

        public CreateTransactionResponse(Transaction entity)
        {
            Id = entity.Id;
            Date = entity.Date;
            Description = entity.Description;
            FromAccountId = entity.FromAccountId;
            Amount = entity.Amount;
        }
    }
}