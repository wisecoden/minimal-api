using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Domain.ViewModels;
using MinimalApi.DTOs;
using Test.Helpers;

namespace Test.Resquest;

[TestClass]
public class AdministratorRequestTest
{
      [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

  [TestMethod]
 public async Task TestGetAndSetProperties()
  {
      // Arrange
      var loginDTO = new LoginDTO
      {
        Email = "adm@teste.com",
        Password = "password"
      };

      var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8,  "Application/json");

      // Act
      var response = await Setup.client.PostAsync("/admins/login", content);

      // Assert
      Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

      var result = await response.Content.ReadAsStringAsync();
      var loggedInAdmin = JsonSerializer.Deserialize<LoggedAdministratorViewModel>(result, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });

      Assert.IsNotNull(loggedInAdmin?.Email ?? "");
      Assert.IsNotNull(loggedInAdmin?.Profile ?? "");
      Assert.IsNotNull(loggedInAdmin?.Token ?? "");

      Console.WriteLine(loggedInAdmin?.Token);

  }
}
