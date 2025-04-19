using Fin.Api.Data;
using Fin.Api.Models;
using Fin.Api.Repository;

namespace Fin.Api.Services;

public class ConfigService(IConfigRepository repository)
{
    private readonly IConfigRepository _repository = repository;
    
    public List<Config> GetAllActive()
    {
        return _repository.GetAllActive();
    }

    public Config Upsert(Config config)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config), "Config cannot be null");
        }

        var configUpserted = _repository.Upsert(config);
        return configUpserted;
    }
}
