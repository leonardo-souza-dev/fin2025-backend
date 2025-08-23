// using Fin.Domain.Entities;
// using Fin.Domain.Interfaces;
// using Fin.Api.Services;
// using Moq;
//
// namespace Fin.Api.Tests.Services;
//
// public class ConfigServiceTests
// {
//     private Mock<IConfigRepository> _configRepositoryMock;
//     private ConfigService _configService;
//
//     [SetUp]
//     public void Setup()
//     {
//         _configRepositoryMock = new Mock<IConfigRepository>();
//         _configService = new ConfigService(_configRepositoryMock.Object);
//     }
//
//     [Test]
//     public void WhenPassingExistingConfigWithSameIdAndKey_ShouldUpdate()
//     {
//         // Arrange
//         var config = new Config
//         {
//             Id = 1333,
//             Key = "TestKey",
//             Value = "TestValue",
//             IsActive = true
//         };
//         _configRepositoryMock.Setup(repo => repo.GetAll())
//             .Returns([new Config { Id = 1333, Key = "TestKey", Value = "OldValue" }]);
//
//         // Act
//         _configService.Update(config);
//
//         // Assert
//         _configRepositoryMock.Verify(repo => repo.Update(It.Is<Config>(c => c == config)), Times.Once);
//     }
//
//     [Test]
//     public void WhenPassingExistingConfigWithSameIdAndDifferentKey_ShouldNotUpdate()
//     {
//         // Arrange
//         var config = new Config
//         {
//             Id = 1333,
//             Key = "TestKey",
//             Value = "TestValue",
//             IsActive = true
//         };
//         _configRepositoryMock.Setup(repo => repo.GetAll())
//             .Returns([new Config { Id = 1333, Key = "another", Value = "OldValue" }]);
//
//         // Act
//         // Assert
//         var ex = Assert.Throws<InvalidOperationException>(() => _configService.Update(config));
//         Assert.True(ex.Message.Equals("Configs must have same key."));
//         _configRepositoryMock.Verify(repo => repo.Update(It.Is<Config>(c => c == config)), Times.Never);
//     }
//
// }
//
