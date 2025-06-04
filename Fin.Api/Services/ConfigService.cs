using Fin.Api.Models;
using Fin.Api.Repository;

namespace Fin.Api.Services;

public class ConfigService(IConfigRepository repository)
{
    private readonly IConfigRepository _repository = repository;
    
    public List<Config> GetAll()
    {
        return _repository.GetAll().ToList();
    }

    public void Create(Config config)
    {
        var existingConfigValue = _repository.GetAll().FirstOrDefault(c => c.Key == config.Key);

        if (existingConfigValue != null)
        {
            throw new InvalidOperationException("There is a config with the same key.");
        }

        _repository.Create(config);
    }

    public void Update(Config config)
    {
        var allConfigs = _repository.GetAll();
        var existingConfigId = allConfigs.First(c => c.Id == config.Id) ?? throw new ArgumentOutOfRangeException("Config not found");

        if (existingConfigId.Key != config.Key)
        {
            throw new InvalidOperationException("Configs must have same key.");
        }

        _repository.Update(config);
    }
}
