using Fin.Domain.Entities;
using Fin.Infrastructure.Data;

namespace Fin.Infrastructure.Repositories
{
    public interface IAccountRepository
    {
        IQueryable<Account> GetAll();
    }

    public class AccountRepository(FinDbContext context) : IAccountRepository
    {
        public IQueryable<Account> GetAll()
        {
            var accounts = context.Accounts.Where(a => a.IsActive);
            return accounts;
        }
    }
}