using Fin.Domain.Entities;

namespace Fin.Infrastructure.Interfaces;

public interface ITransactionRepository
{
    Task Create(Transaction transaction);
}