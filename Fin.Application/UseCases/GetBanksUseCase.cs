using System.Collections;
using Fin.Domain.Entities;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases
{
    public class GetBanksUseCase(IBankRepository bankRepository)
    {
        public GetBanksResponse Handle()
        {
            var banksQuery = bankRepository.GetAll();
            var banks = banksQuery.ToList();
            var response = new GetBanksResponse(banks);

            return response;
        }
    }

    public class GetBanksResponse(IEnumerable<Bank> banks) : IEnumerable<Bank>
    {
        public IEnumerator<Bank> GetEnumerator()
        {
            return banks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}