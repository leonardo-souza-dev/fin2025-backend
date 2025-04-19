using Fin.Api.Data;
using Fin.Api.Models;

namespace Fin.Api.Services;

public class ConfigService(FinDbContext context)
{
    private readonly FinDbContext _context = context;
    
    public List<Config> GetAllActive()
    {
        return _context.Configs
            .Where(a => a.IsActive)
            .ToList();
    }

    public void Upsert(Config config, out bool isCreate)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config), "Config cannot be null");
        }

        var existingConfig = _context.Configs
            .FirstOrDefault(c => c.Id == config.Id);

        if (existingConfig != null)
        {
            existingConfig.Key = config.Key;
            existingConfig.Value = config.Value;
            existingConfig.IsActive = config.IsActive;
            _context.Configs.Update(existingConfig);
            isCreate = false;
        }
        else
        {
            _context.Configs.Add(config);
            isCreate = true;
        }

        _context.SaveChanges();
    }
}
