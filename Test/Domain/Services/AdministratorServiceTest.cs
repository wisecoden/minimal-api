using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Services;
using MinimalApi.Infra.Db;

namespace Test.Domain.Entities;

[TestClass]
public class AdministratorServiceTests
{
    private ApplicationDbContext CreateTestDbContext()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var basePath = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        return new ApplicationDbContext(configuration);
    }

    [TestMethod]
    public void ShouldSaveAdministrator()
    {
        // Arrange
        var context = CreateTestDbContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

        var administrator = new Administrator
        {
            Email = "test@test.com",
            Password = "test",
            Profile = "Admin"
        };

        var service = new AdministratorService(context);

        // Act
        service.Add(administrator);

        // Assert
        Assert.AreEqual(2, service.GetAllAdministrators(1).Count());
    }

    [TestMethod]
    public void ShouldFindAdministratorById()
    {
        // Arrange
        var context = CreateTestDbContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

        var administrator = new Administrator
        {
            Email = "test@test.com",
            Password = "test",
            Profile = "Admin"
        };

        var service = new AdministratorService(context);

        // Act
        service.Add(administrator);
        var result = service.GetAdministratorById(administrator.Id);

        // Assert
        Assert.AreEqual(1, result?.Id);
    }
}
