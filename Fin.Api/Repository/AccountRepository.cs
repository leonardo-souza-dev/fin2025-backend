using Fin.Api.Data;
using Fin.Api.Models;

namespace Fin.Api.Repository;

public class AccountRepository(FinDbContext context) : IAccountRepository
{
    public List<Account> GetAll()
    {
        return [.. context.Accounts.Where(a => a.IsActive)];
    }
}

public interface IAccountRepository
{
    List<Account> GetAll();
}
