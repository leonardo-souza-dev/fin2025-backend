using System.Collections;
using Fin.Domain.Entities;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases.BankAccounts
{
    public class GetAllAccountsUseCase(IAccountRepository accountRepository)
    {
        public GetAllAccountsResponse Handle()
        {
            var accounts = accountRepository.GetAll();
            return GetAllAccountsResponse.Of(accounts);
        }
    }

    public class GetAllAccountsResponse(IEnumerable<Account> accounts) : IEnumerable<Account>
    {
        public IEnumerator<Account> GetEnumerator()
        {
            return accounts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static GetAllAccountsResponse Of(IQueryable<Account> query)
        {
            return new GetAllAccountsResponse(query.ToList());
        }
    }
}