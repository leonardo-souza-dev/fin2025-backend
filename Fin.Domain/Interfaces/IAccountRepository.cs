using Fin.Domain.Entities;

namespace Fin.Domain.Interfaces;

public interface IAccountRepository
{
    IEnumerable<Account> GetAll();
}
