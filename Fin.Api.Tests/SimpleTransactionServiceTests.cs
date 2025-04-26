using Fin.Api.Models;
using Fin.Api.Repository;
using Fin.Api.Services;
using Moq;

namespace Fin.Api.Tests;

public class SimpleTransactionServiceTests
{
    private Mock<ISimpleTransactionRepository> _repositoryMock;

    private SimpleTransactionService _service;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<ISimpleTransactionRepository>();

        _service = new SimpleTransactionService(_repositoryMock.Object);
    }

    [Test]
    public void WhenPassingValidTransactionForCreation_ShouldCreate()
    {
        // Arrange
        var transactionRequest = new Transaction
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            Description = "Test Transaction",
            RefAccountId = 1,
            Amount = 100.00m,
            IsRecurrent = false,
            IsActive = true
        };

        _repositoryMock.Setup(x => x.Create(It.IsAny<SimpleTransaction>()))
            .Callback<SimpleTransaction>(x => x.Id = 1); // Simulate setting the Id after creation

        // Act
        var result = _service.Upsert(transactionRequest);

        // Assert
        Assert.That(result, Is.Not.Null);
        _repositoryMock.Verify(x => x.Create(It.IsAny<SimpleTransaction>()), Times.Once);
        _repositoryMock.Verify(x => x.Update(It.IsAny<SimpleTransaction>()), Times.Never);
    }

    [Test]
    public void WhenPassingValidTransactionForUpdate_ShouldUpdate()
    {
        // Arrange
        var transactionRequest = new Transaction
        {
            Id = 1001,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Description = "Test Transaction",
            RefAccountId = 1,
            Amount = 100.00m,
            IsRecurrent = false,
            IsActive = true
        };

        _repositoryMock.Setup(x => x.Create(It.IsAny<SimpleTransaction>()));

        // Act
        var result = _service.Upsert(transactionRequest);

        // Assert
        Assert.That(result, Is.Not.Null);
        _repositoryMock.Verify(x => x.Update(It.IsAny<SimpleTransaction>()), Times.Once);
        _repositoryMock.Verify(x => x.Create(It.IsAny<SimpleTransaction>()), Times.Never);
    }
}

