using Fin.Api.Data;
using Fin.Api.Models;

namespace Fin.Api.Repository;

public class SimpleTransactionRepository(FinDbContext context) : ISimpleTransactionRepository
{
    private readonly FinDbContext _context = context;

    public List<SimpleTransaction> GetAll()
    {
        return [.. _context.SimpleTransactions.Where(a => a.IsActive)];
    }

    public void Create(SimpleTransaction simpleTransaction)
    {
        var existingEntity = _context.SimpleTransactions
            .FirstOrDefault(c => c.Id == simpleTransaction.Id);

        if (existingEntity != null)
        {
            throw new Exception("SimpleTransaction already exists");
        }
        
        simpleTransaction.IsActive = true;
        _context.SimpleTransactions.Add(simpleTransaction);
        
        _context.SaveChanges();
    }

    public void Update(SimpleTransaction simpleTransaction)
    {
        var existingEntity = _context.SimpleTransactions
            .FirstOrDefault(c => c.Id == simpleTransaction.Id);

        if (existingEntity == null)
        {
            throw new Exception("SimpleTransaction not found");
        }

        existingEntity.Date = simpleTransaction.Date;
        existingEntity.Description = simpleTransaction.Description;
        existingEntity.AccountId = simpleTransaction.AccountId;
        existingEntity.Amount = simpleTransaction.Amount;
        existingEntity.IsRecurrent = simpleTransaction.IsRecurrent;
        existingEntity.IsActive = true;

        _context.SimpleTransactions.Update(existingEntity);
        
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var simpleTransaction = _context.SimpleTransactions
            .FirstOrDefault(c => c.Id == id);

        if (simpleTransaction != null)
        {
            simpleTransaction.IsActive = false;
            _context.SimpleTransactions.Update(simpleTransaction);
            _context.SaveChanges();
        }
    }
}

public interface ISimpleTransactionRepository
{
    List<SimpleTransaction> GetAll();
    void Create(SimpleTransaction simpleTransaction);
    void Update(SimpleTransaction simpleTransaction);
    void Delete(int id);
}
