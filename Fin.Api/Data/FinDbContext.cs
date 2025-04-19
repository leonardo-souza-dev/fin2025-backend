using Fin.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fin.Api.Data;

public class FinDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<Config> Configs { get; set; }
    public DbSet<SimpleTransaction> SimpleTransactions { get; set; }
    public DbSet<Recurrence> Recurrences { get; set; }
    public DbSet<Transfer> Transfers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().ToTable("accounts");
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Bank>().ToTable("banks");
        modelBuilder.Entity<Config>().ToTable("configs");
        modelBuilder.Entity<SimpleTransaction>().ToTable("simpleTransactions");
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string databasePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "fin_db.db");
        optionsBuilder.UseSqlite($"Data Source={databasePath}");
    }
}