using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Fin.Domain.Entities;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases.BankAccounts
{
    public class GetAllBankAccountsUseCase(
        IAccountRepository accountRepository,
        IBankRepository bankRepository)
    {
        public GetAllBankAccountsResponse Handle()
        {
            var accounts = accountRepository.GetAll();
            var banks = bankRepository.GetAll();
            var response = new GetAllBankAccountsResponse(accounts, banks);

            return response;
        }
    }

    public class GetAllBankAccountsResponse : Dictionary<string, List<Account>>
    {
        public GetAllBankAccountsResponse(IEnumerable<Account> accounts, IEnumerable<Bank> banks)
        {
            var bankDictionary = banks.ToDictionary(b => b.Id, b => b.Name);
            foreach (var account in accounts)
            {
                if (!bankDictionary.TryGetValue(account.BankId, out var bankName)) 
                    continue;
                
                if (!this.ContainsKey(bankName))
                {
                    this[bankName] = new List<Account>();
                }

                this[bankName].Add(account);
            }
        }
    }
}