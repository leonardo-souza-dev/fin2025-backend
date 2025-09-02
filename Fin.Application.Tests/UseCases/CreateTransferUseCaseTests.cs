using Fin.Domain.Entities;
using Fin.Application.UseCases;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace Fin.Application.Tests.UseCases;

public class CreateTransferUseCaseTests
{
    private Mock<IPaymentRepository> _paymentRepositoryMock;
    private Mock<ITransferRepository> _transferRepositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    
    private Mock<IDbContextTransaction> _dbContextTransactionMock;

    private CreateTransferUseCase _sut;

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

        _sut = new CreateTransferUseCase(
            _paymentRepositoryMock.Object, 
            _transferRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Test]
    public void WhenPassingValidData_ShouldCreateTransfer()
    {
        // Arrange
        var paymentFromCreated = new Payment
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
        var paymentToCreated = new Payment
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
        
        _paymentRepositoryMock
            .Setup(x => x.Create(It.Is<Payment>(t => t.Amount == -100)))
            .Returns(paymentFromCreated);
        _paymentRepositoryMock
            .Setup(x => x.Create(It.Is<Payment>(t => t.Amount == 100)))
            .Returns(paymentToCreated);
        
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
        _paymentRepositoryMock.Verify(x => x.Create(It.IsAny<Payment>()), Times.Exactly(2));
        _unitOfWorkMock.Verify(x => x.SaveChanges(), Times.Exactly(2));
        _transferRepositoryMock.Verify(x => x.Create(It.IsAny<Transfer>()), Times.Once);
        _dbContextTransactionMock.Verify(x => x.Commit(), Times.Once);
    }
}

