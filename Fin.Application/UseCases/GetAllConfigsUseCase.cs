using System.Collections;
using Fin.Domain.Entities;
using Fin.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;

namespace Fin.Application.UseCases;

public class GetAllConfigsUseCase(IConfigRepository repository)
{
    public GetAllConfigsResponse Handle() =>
        GetAllConfigsResponse.Of(repository.GetAll());

    public class GetAllConfigsResponse(IEnumerable<Config> configs) : IEnumerable<Config>
    {
        public IEnumerator<Config> GetEnumerator() => configs.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static GetAllConfigsResponse Of(IEnumerable<Config> configs)
        {
            return new GetAllConfigsResponse(configs);
        }
    }
    
}