using Fin.Domain.Entities;
using Fin.Infrastructure.Data;

namespace Fin.Infrastructure.Repositories
{
    public interface IConfigRepository
    {
        IEnumerable<Config> GetAll();
        Config? Get(int id);
        void Create(Config config);
        void Update(Config config);
    }

    public class ConfigRepository(FinDbContext context) : IConfigRepository
    {
        public IEnumerable<Config> GetAll()
        {
            return context.Configs.Where(a => a.IsActive);
        }

        public Config? Get(int id)
        {
            return context.Configs.Find(id);
        }

        public void Create(Config config)
        {
            config.IsActive = true;
            context.Configs.Add(config);
        }

        public void Update(Config config)
        {
            var existingConfig = context.Configs.Find(config.Id);

            if (existingConfig == null)
            {
                throw new InvalidOperationException();
            }

            existingConfig.Key = config.Key;
            existingConfig.Value = config.Value;
            existingConfig.IsActive = true;
        }
    }
}