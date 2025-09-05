using Fin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fin.Infrastructure.Data;

public sealed class FinDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Bank> Banks => Set<Bank>();
    public DbSet<Config> Configs => Set<Config>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Recurrence> Recurrences => Set<Recurrence>();
    public DbSet<Transfer> Transfers => Set<Transfer>();

    public FinDbContext(DbContextOptions<FinDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().ToTable("accounts");
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Bank>().ToTable("banks");
        modelBuilder.Entity<Config>().ToTable("configs");
        modelBuilder.Entity<Payment>().ToTable("payments");
        modelBuilder.Entity<Recurrence>().ToTable("recurrences");
        modelBuilder.Entity<Transfer>().ToTable("transfers");

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName()?.ToLower());
            }
        }
    }
}