using  MinimalApi.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class AdministratorTest
{
  [TestMethod]
  public void TestGetAndSetProperties()
  {
    // Arrange
    var administrator = new Administrator();

    // Act
    administrator.Id = 1;
    administrator.Email = "teste@teste.com";
    administrator.Password = "password";
    administrator.Profile = "Admin";

    //Assert
    Assert.AreEqual(1, administrator.Id);
    Assert.AreEqual("teste@teste.com", administrator.Email);
    Assert.AreEqual("password", administrator.Password);
    Assert.AreEqual("Admin", administrator.Profile);


  }
}
