using Fin.Api.Models;
using Fin.Api.Repository;
using Fin.Api.Services;
using Moq;

namespace Fin.Api.Tests;

public class ConfigServiceTests
{
    private Mock<IConfigRepository> _configRepositoryMock;
    private ConfigService _configService;

    [SetUp]
    public void Setup()
    {
        _configRepositoryMock = new Mock<IConfigRepository>();
        _configService = new ConfigService(_configRepositoryMock.Object);
    }

    [Test]
    public void WhenPassingNewConfig_ThenCreate()
    {
        // Arrange
        var config = new Config
        {
            Key = "TestKey",
            Value = "TestValue",
            IsActive = true
        };

        // Act
        _configService.Upsert(config);

        // Assert
        _configRepositoryMock.Verify(repo => repo.Upsert(It.Is<Config>(c => c == config)), Times.Once);
    }

}

