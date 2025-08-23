using Microsoft.EntityFrameworkCore.Storage;

namespace Fin.Infrastructure.Data
{
    public interface IUnitOfWork
    {
        IDbContextTransaction BeginTransaction();
        int SaveChanges();
    }

    public sealed class UnitOfWork(FinDbContext context) : IUnitOfWork
    {
        public IDbContextTransaction BeginTransaction()
        {
            return context.Database.BeginTransaction();
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }
    }
}