using Fin.Api.Models;

namespace Fin.Api.Tests.Models;

public class DayTransactionsTests
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
        var transactions = new List<Transaction>
        {
            new() { Id = 1, Date = new DateOnly(2050, 1, 1), Description = "foo", Amount = amount1, FromAccountId = 10, IsActive = true },
            new() { Id = 2, Date = new DateOnly(2050, 1, 1), Description = "bar", Amount = amount2, FromAccountId = 10, IsActive = true }
        };
        var accountsDb = new List<Account>
        {
            new() { Id = 10, Name = "Account1", BankId = 100, IsActive = true },
            new() { Id = 20, Name = "Account2", BankId = 200, IsActive = true }
        };

        // Act
        var dayTransactions = new DayTransactions(initialBalance, date, transactions);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dayTransactions.Day, Is.EqualTo(date));
            Assert.That(dayTransactions.Transactions, Has.Count.EqualTo(transactions.Count));
            Assert.That(dayTransactions.FinalBalance, Is.EqualTo(finalBalance));
        });
    }
}
