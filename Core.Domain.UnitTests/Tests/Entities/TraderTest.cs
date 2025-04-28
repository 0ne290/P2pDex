using Core.Domain.Entities;

namespace Core.Domain.UnitTests.Tests.Entities;

public class TraderTest
{
    [Fact]
    public void New_Success()
    {
        // Arrange
        const int expectedId = 12345;
        const string expectedName = "Test";

        // Act
        var actualTrader = new Trader(expectedId, expectedName);

        // Assert
        Assert.Equal(expectedId, actualTrader.Id);
        Assert.Equal(expectedName, actualTrader.Name);
    }
}