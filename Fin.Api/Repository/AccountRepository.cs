using Fin.Api.Data;
using Fin.Domain.Entities;
using Fin.Domain.Interfaces;

namespace Fin.Api.Repository;

public class AccountRepository(FinDbContext context) : IAccountRepository
{
    public IEnumerable<Account> GetAll()
    {
        return [.. context.Accounts.Where(a => a.IsActive)];
    }
}
