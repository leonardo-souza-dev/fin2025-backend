using Fin.Application.UseCases;
using Fin.Domain.Entities;

namespace Fin.Application.Interfaces;

public interface ICreateTransferHandler
{
    Task<Transfer> Handle(CreateTransferRequest request, CancellationToken cancellationToken = default);
}
