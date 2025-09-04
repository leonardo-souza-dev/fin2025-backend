using System.Collections;
using Fin.Domain.Entities;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases.Banks
{
    public class GetBanksUseCase(IBankRepository bankRepository)
    {
        public GetBanksResponse Handle()
        {
            var banksQuery = bankRepository.GetAll();
            return GetBanksResponse.Of(banksQuery);
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

        public static GetBanksResponse Of(IQueryable<Bank> banks)
        {
            return new GetBanksResponse(banks);
        }
    }
}