using Fin.Domain.Entities;

namespace Fin.Api.Tests.Models;

public class MonthTests
{
    [Test]
    //[TestCase(2024, 12, "16", 0)]
    //[TestCase(2024, 12, "17", 0)]
    //[TestCase(2025, 3,  "16", 1)]
    //[TestCase(2025, 4, "16", 2)]
    [TestCase(2025, 5, "16", 1)]
    //[TestCase(2025, 6, "16", 0)]
    public void ShouldHaveValidValues(int year, int month, string accounts, int dayPaymentsCountExpected)
    {
        // pre-arrange
        var accountIds = accounts.Split(',').Select(int.Parse).ToList();

        // Arrange
        var date = new DateOnly(2050, 1, 1);
        var payments = new List<Payment>
        {
            new() { Id = 1, Date = new DateOnly(2025, 5, 1),  Description = "transfer resgate", FromAccountId = 16, Amount = -1234, ToAccountId = 62,   RecurrenceId = null, IsActive = true },
            new() { Id = 6, Date = new DateOnly(2025, 4, 1),  Description = "simple food",      FromAccountId = 16, Amount = -1234, ToAccountId = null, RecurrenceId = null, IsActive = true },
            new() { Id = 7, Date = new DateOnly(2025, 3, 29), Description = "uber",             FromAccountId = 16, Amount = -50,   ToAccountId = null, RecurrenceId = null, IsActive = true },
            new() { Id = 8, Date = new DateOnly(2025, 4, 10), Description = "uber",             FromAccountId = 16, Amount = -10,   ToAccountId = null, RecurrenceId = null, IsActive = true }
        };
        var accountsDb = new List<Account>
        {
            new() { Id = 16, Name = "debito", BankId = 1, IsActive = true },
            new() { Id = 17, Name = "credito", BankId = 1, IsActive = true }
        };
        var transfers = new List<Transfer>();

        // Act
        var sut = new Month(year, month, accountIds, accountsDb, payments, transfers);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut, Is.Not.Null);
            Assert.That(sut.DayPayments, Is.Not.Null);
            Assert.That(sut.DayPayments, Has.Count.EqualTo(dayPaymentsCountExpected));
        });
    }
}
