using Fin.Application.UseCases;
using Fin.Domain.Entities;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace Fin.Application.Tests.UseCases;

public class UpdateTransferUseCaseTests
{
    private Mock<IPaymentRepository> _paymentRepositoryMock;
    private Mock<ITransferRepository> _transferRepositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    
    private Mock<IDbContextTransaction> _dbContextTransactionMock;
    
    private UpdateTransferUseCase _sut;
    
    [SetUp]
    public void Setup()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _transferRepositoryMock = new Mock<ITransferRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        _dbContextTransactionMock = new Mock<IDbContextTransaction>();
        _unitOfWorkMock
            .Setup(uow => uow.BeginTransaction())
            .Returns(_dbContextTransactionMock.Object);
        
        _sut = new UpdateTransferUseCase(
            _paymentRepositoryMock.Object,
            _transferRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }
    
    [Test]
    public void GivenValidTransfer_WhenUpdate_ShouldUpdateTransfer()
    {
        // Arrange
        var paymentFrom = new Payment
        {
            Id = 1001,
            Amount = -100,
            Date = new DateOnly(2000, 1, 1),
            Description = "some description",
            FromAccountId = 16,
            ToAccountId = 62
        };
        var paymentTo = new Payment
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
            PaymentFromId = 1001,
            PaymentToId = 1002
        }; 
        _transferRepositoryMock
            .Setup(x => x.Get(5001))
            .Returns(transfer);
        _paymentRepositoryMock
            .Setup(x => x.Get(1001))
            .Returns(paymentFrom);
        _paymentRepositoryMock
            .Setup(x => x.Get(1002))
            .Returns(paymentTo);
        
        var request = new UpdateTransferRequest
        {
            Id = 5001,
            PaymentId = 1001,
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