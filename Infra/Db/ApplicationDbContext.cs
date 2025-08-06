using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;

namespace MinimalApi.infra.Db;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configurationAppSettings;
    public ApplicationDbContext(IConfiguration configurationAppSettings)
    {
        _configurationAppSettings = configurationAppSettings;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>().HasData(
            new Administrator
            {
                Id = 1,
                Email = "adm@teste.com",
                Password = "password",
                Profile = "Adm"
            }
       );
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var stringConnection = _configurationAppSettings.GetConnectionString("mysql")?.ToString();
            if (!string.IsNullOrEmpty(stringConnection))
            {
                optionsBuilder.UseMySql(stringConnection,
                ServerVersion.AutoDetect(stringConnection));
            }
        }
    }
    public DbSet<Administrator> Administrators { get; set; } = default!;
    public DbSet<Vehicle> vehicles { get; set; } = default!;
};