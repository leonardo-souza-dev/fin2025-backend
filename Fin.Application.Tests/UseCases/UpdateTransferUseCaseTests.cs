using Fin.Application.UseCases;
using Fin.Domain.Entities;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace Fin.Application.Tests.UseCases;

public class UpdateTransferUseCaseTests
{
    private Mock<ITransactionRepository> _transactionRepositoryMock;
    private Mock<ITransferRepository> _transferRepositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    
    private Mock<IDbContextTransaction> _dbContextTransactionMock;
    
    private UpdateTransferUseCase _sut;
    
    [SetUp]
    public void Setup()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _transferRepositoryMock = new Mock<ITransferRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        _dbContextTransactionMock = new Mock<IDbContextTransaction>();
        _unitOfWorkMock
            .Setup(uow => uow.BeginTransaction())
            .Returns(_dbContextTransactionMock.Object);
        
        _sut = new UpdateTransferUseCase(
            _transactionRepositoryMock.Object,
            _transferRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }
    
    [Test]
    public void GivenValidTransfer_WhenUpdate_ShouldUpdateTransfer()
    {
        // Arrange
        var transactionFrom = new Transaction
        {
            Id = 1001,
            Amount = -100,
            Date = new DateOnly(2000, 1, 1),
            Description = "some description",
            FromAccountId = 16,
            ToAccountId = 62
        };
        var transactionTo = new Transaction
        {
            Id = 1002,
            Amount = 100,
            Date = new DateOnly(2000, 1, 1),
            Description = "some description",
            FromAccountId = 62,
            ToAccountId = 16
        };
        var transfer = new Transfer
        {
            Id = 5001,
            FromTransactionId = 1001,
            ToTransactionId = 1002
        }; 
        _transferRepositoryMock
            .Setup(x => x.Get(5001))
            .Returns(transfer);
        _transactionRepositoryMock
            .Setup(x => x.Get(1001))
            .Returns(transactionFrom);
        _transactionRepositoryMock
            .Setup(x => x.Get(1002))
            .Returns(transactionTo);
        
        var request = new UpdateTransferRequest
        {
            Id = 5001,
            TransactionId = 1001,
            Date = new DateOnly(2000, 1, 1),
            Description = "new description",
            Amount = -100,
            FromAccountId = 16,
            ToAccountId = 62,
            IsRecurrence = false
        };
        
        // Act
        _sut.Handle(request);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChanges(), Times.Once);
    }
    
}