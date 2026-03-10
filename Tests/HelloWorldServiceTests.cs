namespace Tests;

public class HelloWorldServiceTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GetMessage_ReturnsHelloWorld()
    {
        // Arrange
        var service = new HelloWorldService();

        // Act
        string result = service.GetMessage();

        // Assert
        Assert.That(result, Is.EqualTo("Hello, World!"));

    }
}
