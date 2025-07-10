using Fin.Domain.Entities;

namespace Fin.Domain.Interfaces;

public interface IConfigRepository
{
    IEnumerable<Config> GetAll();
    void Create(Config config);
    void Update(Config config);
}
