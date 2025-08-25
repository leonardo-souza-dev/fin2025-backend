using Fin.Domain.Entities;

namespace Fin.Domain.Tests;

public class FinalBalancePreviousMonthTests
{
    [Test]
    [TestCase(2000, 12, "16", 0)]
    [TestCase(2024, 12, "16", 0)]
    [TestCase(2024, 12, "17", 0)]
    [TestCase(2025, 3, "16", 0)]
    [TestCase(2025, 4, "16", 0)]
    [TestCase(2025, 5, "16", -1234)]
    [TestCase(2025, 6, "16", -2468)]
    [TestCase(2025, 7, "16", -2488)]
    [TestCase(2025, 8, "16,17,62", -1481)]
    [TestCase(2025, 8, "62", 1234)]
    public void ShouldHaveValidValues(int year, int month, string accounts, int finalBalancePreviousMonthExpected)
    {
        // pre-arrange
        var accountIds = accounts.Split(',').Select(int.Parse).ToList();

        // Arrange
        var date = new DateOnly(2050, 1, 1);
        var payments = new List<Payment>
        {
            new() { Id = 1,  Date = new DateOnly(2025, 5, 1),  Description = "transfer resgate", FromAccountId = 16, Amount = -1234, ToAccountId = 62,   RecurrenceId = null, IsActive = true },
            new() { Id = 2,  Date = new DateOnly(2025, 5, 1),  Description = "transfer resgate", FromAccountId = 62, Amount = 1234,  ToAccountId = 16,   RecurrenceId = null, IsActive = true },
            new() { Id = 3,  Date = new DateOnly(2025, 4, 10), Description = "recurrence max",   FromAccountId = 17, Amount = -49,   ToAccountId = null, RecurrenceId = 1, IsActive = true },
            new() { Id = 4,  Date = new DateOnly(2025, 5, 10), Description = "recurrence max",   FromAccountId = 17, Amount = -49,   ToAccountId = null, RecurrenceId = 1, IsActive = true },
            new() { Id = 5,  Date = new DateOnly(2025, 6, 10), Description = "recurrence max",   FromAccountId = 17, Amount = -49,   ToAccountId = null, RecurrenceId = 1, IsActive = true },
            new() { Id = 6,  Date = new DateOnly(2025, 4, 1),  Description = "simple food",      FromAccountId = 16, Amount = -1234, ToAccountId = null, RecurrenceId = null, IsActive = true },
            new() { Id = 7,  Date = new DateOnly(2025, 3, 29), Description = "uber",             FromAccountId = 17, Amount = -50,   ToAccountId = null, RecurrenceId = null, IsActive = true },
            new() { Id = 8,  Date = new DateOnly(2025, 5, 10), Description = "netflix",          FromAccountId = 17, Amount = -10,   ToAccountId = null, RecurrenceId = 2, IsActive = true },
            new() { Id = 9,  Date = new DateOnly(2025, 6, 10), Description = "netflix",          FromAccountId = 17, Amount = -10,   ToAccountId = null, RecurrenceId = 2, IsActive = true },
            new() { Id = 10, Date = new DateOnly(2025, 7, 10), Description = "netflix",          FromAccountId = 17, Amount = -10,   ToAccountId = null, RecurrenceId = 2, IsActive = true },
            new() { Id = 11, Date = new DateOnly(2025, 6, 10), Description = "mercado",          FromAccountId = 16, Amount = -20,   ToAccountId = null, RecurrenceId = null, IsActive = true }
        };
        var accountsDb = new List<Account>
        {
            new() { Id = 16, Name = "debito", BankId = 1, IsActive = true },
            new() { Id = 17, Name = "credito", BankId = 1, IsActive = true },
            new() { Id = 62, Name = "itau", BankId = 2, IsActive = true }
        };

        // Act
        var sut = new FinalBalancePreviousMonth(year, month, accountIds, accountsDb, payments);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut, Is.Not.Null);
            Assert.That(sut.Value, Is.EqualTo(finalBalancePreviousMonthExpected));
        });
    }
}
