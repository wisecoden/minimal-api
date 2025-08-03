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
     public DbSet<Admin> Admins { get; set; } = default!;
};