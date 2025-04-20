using Fin.Api.Data;
using Fin.Api.Models;
using Fin.Api.Repository;
using System.Threading.Tasks;

namespace Fin.Api.Services;

public class SimpleTransactionService(ISimpleTransactionRepository repository)
{
    private readonly ISimpleTransactionRepository _repository = repository;

    public List<SimpleTransaction> GetAllActive()
    {
        return _repository.GetAll();
    }

    public Transaction Upsert(Transaction transactionRequest)
    {
        SimpleTransaction simpleTransactionUpserting;

        var isCreate = transactionRequest.Id == 0 || transactionRequest.Id == null;

        if (isCreate)
        {
            simpleTransactionUpserting = new SimpleTransaction
            {
                Date = transactionRequest.Date,
                Description = transactionRequest.Description,
                AccountId = transactionRequest.RefAccountId,
                Amount = transactionRequest.Amount,
                IsRecurrent = transactionRequest.IsRecurrent,
                IsActive = transactionRequest.IsActive
            };

            _repository.Create(simpleTransactionUpserting);
        }
        else
        {
            simpleTransactionUpserting = new SimpleTransaction
            {
                Id = transactionRequest.Id.Value,
                Date = transactionRequest.Date,
                Description = transactionRequest.Description,
                AccountId = transactionRequest.RefAccountId,
                Amount = transactionRequest.Amount,
                IsRecurrent = transactionRequest.IsRecurrent,
                IsActive = transactionRequest.IsActive
            };
            
            _repository.Update(simpleTransactionUpserting);            
        }

        return new Transaction
        {
            Id = simpleTransactionUpserting.Id,
            Date = transactionRequest.Date,
            Description = transactionRequest.Description,
            RefAccountId = transactionRequest.RefAccountId,
            Amount = transactionRequest.Amount,
            Type = "SIMPLE",
            OtherAccountId = transactionRequest.OtherAccountId,
            IsRecurrent = transactionRequest.IsRecurrent,
            IsActive = transactionRequest.IsActive
        };
    }

    public void Delete(int id)
    {
        _repository.Delete(id);
    }
}
