using Fin.Api.Data;
using Fin.Api.Models;

namespace Fin.Api.Repository;

public class ConfigRepository(FinDbContext context) : IConfigRepository
{
    public IEnumerable<Config> GetAll()
    {
        return context.Configs.Where(a => a.IsActive);
    }

    public void Create(Config config)
    {
        config.IsActive = true;
        context.Configs.Add(config);
        context.SaveChanges();
    }

    public void Update(Config config)
    {
        var existingConfig = context.Configs
            .FirstOrDefault(c => c.Id == config.Id);

        if (existingConfig == null)
        {
            throw new InvalidOperationException();
        }
        existingConfig.Key = config.Key;
        existingConfig.Value = config.Value;
        existingConfig.IsActive = true;
        context.Configs.Update(existingConfig);
        context.SaveChanges();
    }
}

public interface IConfigRepository
{
    IEnumerable<Config> GetAll();
    void Create(Config config);
    void Update(Config config);
}
