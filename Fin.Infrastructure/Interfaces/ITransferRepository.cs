using Fin.Domain.Entities;

namespace Fin.Infrastructure.Interfaces;

public interface ITransferRepository
{
    Task Create(Transfer transfer);
}