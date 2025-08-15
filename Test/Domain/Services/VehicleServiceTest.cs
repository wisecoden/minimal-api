using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Services;
using MinimalApi.Infra.Db;

namespace Test.Domain.Entities;

[TestClass]
public class VehicleServiceTest
{
    private ApplicationDbContext CreateTestDbContext()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new ApplicationDbContext(configuration);
    }

    [TestMethod]
    public void TestSaveVehicle()
    {
        // Arrange
        var context = CreateTestDbContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE vehicles");
        
        

        var vehicle = new Vehicle
        {
            Name = "Test Vehicle",
            Brand = "Test Brand",
            Year = 2023
        };

        var vehicleService = new VehicleService(context);

        // Act
        vehicleService.Add(vehicle);

        // Assert
        Assert.AreEqual(vehicle.Id, vehicleService.GetAllVehicles(1).Count());
    }

    [TestMethod]
    public void TestFindVehicleById()
    {
        // Arrange
        var context = CreateTestDbContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE vehicles");

        var vehicle = new Vehicle
        {
            Name = "Test Vehicle",
            Brand = "Test Brand",
            Year = 2023
        };

        var vehicleService = new VehicleService(context);

        // Act
        vehicleService.Add(vehicle);
        var result = vehicleService.GetVehicleById(vehicle.Id);

        // Assert
        Assert.AreEqual(vehicle.Id, result?.Id);
    }
}
