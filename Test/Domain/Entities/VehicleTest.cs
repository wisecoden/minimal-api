using  MinimalApi.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class VehicleTest
{
  [TestMethod]
  public void TestGetAndSetProperties()
  {
    // Arrange
    var vehicle = new Vehicle();

    // Act
    vehicle.Id = 1;
    vehicle.Name = "Civic";
    vehicle.Brand = "Honda";
    vehicle.Year = 2022;

    //Assert
    Assert.AreEqual(1, vehicle.Id);
    Assert.AreEqual("Civic", vehicle.Name);
    Assert.AreEqual("Honda", vehicle.Brand);
    Assert.AreEqual(2022, vehicle.Year);
  }
}
