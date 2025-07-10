using Fin.Domain.Entities;

namespace Fin.Domain.Interfaces;

public interface ITransactionRepository
{
    IEnumerable<Transaction> GetAll(int? year = null, int? month = null);
    void Create(Transaction transaction);
    void Update(Transaction transaction);
    void Delete(Transaction transaction);
    void DeleteTransfer(Transaction transaction1, Transaction transaction2);
}
