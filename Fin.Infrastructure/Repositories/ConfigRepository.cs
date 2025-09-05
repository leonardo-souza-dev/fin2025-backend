using System.Linq.Expressions;
using Fin.Domain.Entities;
using Fin.Infrastructure.Data;

namespace Fin.Infrastructure.Repositories
{
    public interface IConfigRepository
    {
        IQueryable<Config> GetAll(Expression<Func<Config, bool>>? predicate = null);
        Config? Get(int id);
        void Create(Config config);
        void Update(Config config);
    }

    public class ConfigRepository(FinDbContext context) : IConfigRepository
    {
        public IQueryable<Config> GetAll(Expression<Func<Config, bool>>? predicate = null)
        {
            predicate ??= (_ => true);
            
            return context.Configs
                .Where(c => c.IsActive)
                .Where(predicate).AsQueryable();
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