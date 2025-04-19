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
        var newId = 1001;
        var config = new Config
        {
            Key = "TestKey",
            Value = "TestValue",
            IsActive = true
        };

        _configRepositoryMock
            .Setup(repo => repo.Upsert(config))
            .Returns(() =>
            {
                config.Id = newId; // Simulate the ID being set by the repository
                return config;
            });

        // Act
        var result = _configService.Upsert(config);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Id, Is.EqualTo(newId));
        Assert.That(result.Key, Is.EqualTo(config.Key));
        Assert.That(result.Value, Is.EqualTo(config.Value));
        Assert.That(result.IsActive, Is.EqualTo(config.IsActive));

        _configRepositoryMock.Verify(repo => repo.Upsert(It.Is<Config>(c => c == config)), Times.Once);
    }

}

