using Fin.Domain.Entities;

namespace Fin.Domain.Tests;

public class DayPaymentsTests
{
    [Test]
    [TestCase(-300, 100, 200, 0)]
    [TestCase(-300, -100, 200, -200)]
    [TestCase(0, 100, -200, -100)]
    [TestCase(1000, -500, -499, 1)]
    public void ShouldHaveValidValues(int initialBalance, int amount1, int amount2, int finalBalance)
    {
        // Arrange
        var date = new DateOnly(2050, 1, 1);
        var payments = new List<Payment>
        {
            new() { Id = 1, Date = new DateOnly(2050, 1, 1), Description = "foo", Amount = amount1, FromAccountId = 10, IsActive = true },
            new() { Id = 2, Date = new DateOnly(2050, 1, 1), Description = "bar", Amount = amount2, FromAccountId = 10, IsActive = true },
            new() { Id = 3, Date = new DateOnly(2050, 1, 1), Description = "transfer100", Amount = 100, FromAccountId = 16, IsActive = true, ToAccountId = 62 },
            new() { Id = 4, Date = new DateOnly(2050, 1, 1), Description = "transfer100", Amount = -100, FromAccountId = 62, IsActive = true, ToAccountId = 16 }
        };
        var accountsDb = new List<Account>
        {
            new() { Id = 10, Name = "Account1", BankId = 100, IsActive = true },
            new() { Id = 20, Name = "Account2", BankId = 200, IsActive = true }
        };
        var transfers = new List<Transfer>
        {
            new()
            {
                PaymentFromId = 3,
                IsActive = true,
                PaymentToId = 3
            }
        };

        // Act
        var dayPayments = new DayPayments(initialBalance, date, payments, transfers);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dayPayments.Day, Is.EqualTo(date));
            Assert.That(dayPayments.Payments, Has.Count.EqualTo(payments.Count));
            Assert.That(dayPayments.FinalBalance, Is.EqualTo(finalBalance));
        });
    }
}
