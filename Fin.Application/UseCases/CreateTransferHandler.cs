using Fin.Application.Interfaces;
using Fin.Domain.Entities;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Interfaces;

namespace Fin.Application.UseCases;

public class CreateTransferHandler(
    ITransactionRepository transactionRepository,
    ITransferRepository transferRepository,
    IUnitOfWork unitOfWork) : ICreateTransferHandler
{

    public async Task<Transfer> Handle(CreateTransferRequest request, CancellationToken cancellationToken = default)
    {
        var transactionFrom = new Transaction
        {
            Date = request.Date,
            Description = request.Description,
            FromAccountId = request.FromAccountId,
            Amount = request.Amount,
            ToAccountId = request.ToAccountId,
            IsActive = true
        };
        var transactionTo = new Transaction
        {
            Date = request.Date,
            Description = request.Description,
            FromAccountId = request.ToAccountId,
            Amount = request.Amount * -1,
            ToAccountId = request.FromAccountId,
            IsActive = true
        };
        
        await using var transaction = await unitOfWork.BeginTransactionAsync();

        try
        {
            await transactionRepository.Create(transactionFrom);
            await unitOfWork.SaveChangesAsync();
            
            await transactionRepository.Create(transactionTo);
            await unitOfWork.SaveChangesAsync();

            var transfer = new Transfer
            {
                FromTransactionId = transactionFrom.Id.Value,
                FromTransaction = transactionFrom,
                ToTransactionId = transactionTo.Id.Value,
                ToTransaction = transactionTo,
                IsActive = true
            };
            await transferRepository.Create(transfer);
            await unitOfWork.SaveChangesAsync();
            
            await transaction.CommitAsync();

            return transfer;
        }
        catch (Exception ex) 
        {
            await transaction.RollbackAsync();
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
    public required DateOnly Date { get; set; }
    public required string Description { get; set; }
    public required int FromAccountId { get; set; }
    public required decimal Amount { get; set; }
    public required int ToAccountId { get; set; }
    public required bool IsRecurrence { get; set; }
}
