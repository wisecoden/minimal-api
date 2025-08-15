using Microsoft.EntityFrameworkCore;
using  MinimalApi.Domain.Entities;

namespace MinimalApi.Infra.Db;

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
            new Administrator {
                Id = 1,
                Email = "administrador@teste.com",
                Password = "password",
                Profile = "Admin"
             }
        );
       
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var stringConnection = _configurationAppSettings.GetConnectionString("MySql")?.ToString();
            if (!string.IsNullOrEmpty(stringConnection))
            {
                optionsBuilder.UseMySql(stringConnection,
                ServerVersion.AutoDetect(stringConnection));
            }
        }
    }
    public DbSet<Administrator> Administrators { get; set; } = default!;
    public DbSet<Vehicle> Vehicles { get; set; } = default!;
};