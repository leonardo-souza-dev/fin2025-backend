using System.Collections;
using Fin.Domain.Entities;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases
{
    public class GetAllAccountsUseCase(IAccountRepository accountRepository)
    {
        public GetAllAccountsResponse Handle()
        {
            var accountsQuery = accountRepository.GetAll();
            var accounts = accountsQuery.ToList();
            var response = new GetAllAccountsResponse(accounts);

            return response;
        }
    }

    public class GetAllAccountsResponse(IEnumerable<Account> accounts) : IEnumerable<Account>
    {
        public IEnumerator<Account> GetEnumerator() => accounts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}