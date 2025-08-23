using Fin.Domain.Entities;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases;

public class ConfigService(IConfigRepository repository)
{
    public List<Config> GetAll()
    {
        return repository.GetAll().ToList();
    }

    public void Create(Config config)
    {
        var existingConfigValue = repository.GetAll().FirstOrDefault(c => c.Key == config.Key);

        if (existingConfigValue != null)
        {
            throw new InvalidOperationException("There is a config with the same key.");
        }

        repository.Create(config);
    }

    public void Update(Config config)
    {
        var allConfigs = repository.GetAll();
        var existingConfigId = allConfigs.First(c => c.Id == config.Id) ?? throw new ArgumentOutOfRangeException("Config not found");

        if (existingConfigId.Key != config.Key)
        {
            throw new InvalidOperationException("Configs must have same key.");
        }

        repository.Update(config);
    }
}
