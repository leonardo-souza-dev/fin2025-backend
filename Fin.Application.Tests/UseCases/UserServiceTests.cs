using Fin.Application.UseCases;
using Fin.Domain.Entities;
using Fin.Domain.Exceptions;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;
using Moq;

namespace Fin.Application.Tests.UseCases;

public class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;

    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _userService = new UserService(_userRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Test]
    public void WhenPassingExistingEmail_ShouldGetUser()
    {
        // Arrange

        var email = "john@email.com";
        var users = new List<User>
        {
            new() { Id = 1, Email = email, IsActive = true, Password = "123456789", Role = "user" },
            new() { Id = 2, Email = "alice@email.com", IsActive = true, Password = "123456789", Role = "user" }
        }.AsQueryable();

        _userRepositoryMock
            .Setup(x => x.GetUserByEmail(email))
            .Returns(users.FirstOrDefault(u => u.Email == email));

        // Act
        var result = _userService.GetUserByEmail(email);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<User>());
        Assert.That(result.Email, Is.EqualTo(email));
    }
}

