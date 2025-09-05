using Fin.Domain.Entities;
using Fin.Infrastructure.Data;

namespace Fin.Infrastructure.Repositories
{
    public interface IBankRepository
    {
        IQueryable<Bank> GetAll();
    }

    public class BankRepository(FinDbContext context) : IBankRepository
    {
        public IQueryable<Bank> GetAll()
        {
            var banks = context.Banks.Where(a => a.IsActive);
            return banks;
        }
    }
}