using Fin.Api.Infra;
using Fin.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Fin.Api.Data;

public class FinDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<Config> Configs { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Recurrence> Recurrences { get; set; }
    public DbSet<Transfer> Transfers { get; set; }

    private readonly ServerPortInfraService _serverPortInfraService;
    private readonly IConfiguration _configuration;

    public FinDbContext(
        ServerPortInfraService serverPortInfraService,
        IConfiguration configuration)
    {
        _serverPortInfraService = serverPortInfraService;
        _configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().ToTable("accounts");
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Bank>().ToTable("banks");
        modelBuilder.Entity<Config>().ToTable("configs");
        modelBuilder.Entity<Transaction>().ToTable("transactions");
        modelBuilder.Entity<Recurrence>().ToTable("recurrences");
        modelBuilder.Entity<Transfer>().ToTable("transfers");

        modelBuilder.Entity<Transfer>()
            .HasKey(t => new { t.FromTransactionId, t.ToTransactionId });

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
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = GetConnectionString();
            optionsBuilder.UseSqlite(connectionString);
        }
    }

    private string GetConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable("FIN2025_DATABASE_CONNECTION");

        if (!string.IsNullOrEmpty(connectionString))
        {
            return connectionString;
        }
        
        throw new InvalidOperationException(
            "String de conexão não configurada. Configure a variável de ambiente FIN2025_DATABASE_CONNECTION ou a configuração DefaultConnection.");
    }
}
