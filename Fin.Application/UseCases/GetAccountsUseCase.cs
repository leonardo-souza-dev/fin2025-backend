using System.Collections;
using Fin.Domain.Entities;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases
{
    public interface IGetAccountsUseCase
    {
        GetAccountsResponse Handle();
    }
    
    public class GetAccountsUseCase(IAccountRepository accountRepository) : IGetAccountsUseCase
    {
        public GetAccountsResponse Handle()
        {
            var accountsQuery = accountRepository.GetAll();
            var accounts = accountsQuery.ToList();
            var response = new GetAccountsResponse(accounts);

            return response;
        }
    }

    public class GetAccountsResponse(IEnumerable<Account> accounts) : IEnumerable<Account>
    {
        public IEnumerator<Account> GetEnumerator()
        {
            return accounts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}