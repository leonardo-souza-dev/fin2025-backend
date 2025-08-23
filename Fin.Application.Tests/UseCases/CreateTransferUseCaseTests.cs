using Fin.Domain.Entities;
using Fin.Application.UseCases;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace Fin.Application.Tests.UseCases;

public class CreateTransferUseCaseTests
{
    private Mock<Fin.Infrastructure.Repositories.ITransactionRepository> _transactionRepositoryMock;
    private Mock<Fin.Infrastructure.Repositories.ITransferRepository> _transferRepositoryMock;
    private Mock<Fin.Infrastructure.Data.IUnitOfWork> _unitOfWorkMock;
    
    private Mock<IDbContextTransaction> _dbContextTransactionMock;

    private CreateTransferUseCase _sut;

    [SetUp]
    public void Setup()
    {
        _transactionRepositoryMock = new Mock<Fin.Infrastructure.Repositories.ITransactionRepository>();
        _transferRepositoryMock = new Mock<Fin.Infrastructure.Repositories.ITransferRepository>();
        _unitOfWorkMock = new Mock<Fin.Infrastructure.Data.IUnitOfWork>();
        
        _dbContextTransactionMock = new Mock<IDbContextTransaction>();
        _unitOfWorkMock
            .Setup(uow => uow.BeginTransaction())
            .Returns(_dbContextTransactionMock.Object);

        _sut = new CreateTransferUseCase(
            _transactionRepositoryMock.Object, 
            _transferRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Test]
    public void WhenPassingValidData_ShouldCreateTransfer()
    {
        // Arrange
        var transactionFromCreated = new Transaction
        {
            Id = 1001,
            Date = new DateOnly(2020, 1, 1),
            Description = "transfer100",
            FromAccountId = 16,
            Amount = -100,
            ToAccountId = 62,
            RecurrenceId = null,
            IsActive = true,
            TransferId = 5001
        };
        var transactionToCreated = new Transaction
        {
            Id = 1002,
            Date = new DateOnly(2020, 1, 1),
            Description = "transfer100",
            FromAccountId = 62,
            Amount = 100,
            ToAccountId = 16,
            RecurrenceId = null,
            IsActive = true,
            TransferId = 5001
        };
        
        _transactionRepositoryMock
            .Setup(x => x.Create(It.Is<Transaction>(t => t.Amount == -100)))
            .Returns(transactionFromCreated);
        _transactionRepositoryMock
            .Setup(x => x.Create(It.Is<Transaction>(t => t.Amount == 100)))
            .Returns(transactionToCreated);
        
        var request = new CreateTransferRequest
        {
            Date = new DateOnly(2000, 1, 1),
            Description = "transfer100",
            FromAccountId = 16,
            Amount = -100,
            ToAccountId = 62,
            IsRecurrence = false
        };
        
        // Act
        _sut.Handle(request);

        // Assert
        _unitOfWorkMock.Verify(x => x.BeginTransaction(), Times.Once);
        _transactionRepositoryMock.Verify(x => x.Create(It.IsAny<Transaction>()), Times.Exactly(2));
        _unitOfWorkMock.Verify(x => x.SaveChanges(), Times.Once);
        _transferRepositoryMock.Verify(x => x.Create(It.IsAny<Transfer>()), Times.Once);
        _dbContextTransactionMock.Verify(x => x.Commit(), Times.Once);
    }
}

