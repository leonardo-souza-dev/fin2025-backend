using Fin.Api.Data;
using Fin.Api.Models;

namespace Fin.Api.Repository;

public class ConfigRepository(FinDbContext context) : IConfigRepository
{
    private readonly FinDbContext _context = context;

    public List<Config> GetAllActive()
    {
        return [.. _context.Configs.Where(a => a.IsActive)];
    }

    public Config Upsert(Config config)
    {
        var existingConfig = _context.Configs
            .FirstOrDefault(c => c.Id == config.Id);

        if (existingConfig != null)
        {
            existingConfig.Key = config.Key;
            existingConfig.Value = config.Value;
            existingConfig.IsActive = config.IsActive;
            _context.Configs.Update(existingConfig);
        }
        else
        {
            _context.Configs.Add(config);
        }
        _context.SaveChanges();

        return config;
    }
}

public interface IConfigRepository
{
    List<Config> GetAllActive();
    Config Upsert(Config config);
}
