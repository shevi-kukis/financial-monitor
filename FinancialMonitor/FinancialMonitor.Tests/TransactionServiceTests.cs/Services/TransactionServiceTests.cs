using FinancialMonitor.Data;
using FinancialMonitor.DTO;
using FinancialMonitor.Hubs;
using FinancialMonitor.Models;
using FinancialMonitor.Services;
using FinancialMonitor.Tests.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace FinancialMonitor.Tests.Services;

public class TransactionServiceTests
{
    private TransactionService CreateService(
        AppDbContext context,
        Mock<IClientProxy>? clientProxyMock = null)
    {
        var hubContextMock = new Mock<IHubContext<TransactionHub>>();
        var clientsMock = new Mock<IHubClients>();
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();

        clientProxyMock ??= new Mock<IClientProxy>();

        hubContextMock
            .Setup(h => h.Clients)
            .Returns(clientsMock.Object);

        clientsMock
            .Setup(c => c.All)
            .Returns(clientProxyMock.Object);

        clientProxyMock
            .Setup(c => c.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object[]>(),
                default))
            .Returns(Task.CompletedTask);

        return new TransactionService(
            context,
            hubContextMock.Object,
            scopeFactoryMock.Object);
    }

    [Fact]
    public async Task AddTransaction_Should_Save_To_Database()
    {
        var context = TestDbContextFactory.Create();
        var service = CreateService(context);

        await service.AddTransactionAsync(
            new TransactionDto(10m, "USD"));

        var count = await context.Transactions.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task AddTransaction_Should_Handle_Concurrent_Calls_Safely()
    {
        var context = TestDbContextFactory.Create();
        var service = CreateService(context);

        const int numberOfTransactions = 100;

        var tasks = Enumerable.Range(0, numberOfTransactions)
            .Select(_ => service.AddTransactionAsync(
                new TransactionDto(20m, "USD")));

        await Task.WhenAll(tasks);

        var transactions = await context.Transactions.ToListAsync();

        Assert.Equal(numberOfTransactions, transactions.Count);

        Assert.Equal(
            numberOfTransactions,
            transactions.Select(t => t.Id).Distinct().Count());


        Assert.All(transactions,
            t => Assert.Equal(TransactionStatus.Pending, t.Status));
    }
    [Fact]
    public async Task GetAllAsync_Should_Return_Transactions_Ordered_By_CreatedAt()
    {
        var context = TestDbContextFactory.Create();
        var service = CreateService(context);

        await service.AddTransactionAsync(
            new TransactionDto(1m, "USD"));

        await Task.Delay(10);

        await service.AddTransactionAsync(
            new TransactionDto(2m, "USD"));

        var results = await service.GetAllAsync();

        Assert.Equal(2, results.Count());
        Assert.Equal(2, results.First().Amount);
    }


    [Fact]
    public async Task AddTransaction_Should_Preserve_Input_Data()
    {
        var context = TestDbContextFactory.Create();
        var service = CreateService(context);

        var dto = new TransactionDto(99m, "EUR");

        var result = await service.AddTransactionAsync(dto);

        Assert.Equal(99m, result.Amount);
        Assert.Equal("EUR", result.Currency);
    }

    [Fact]
    public async Task AddTransaction_Should_Broadcast_On_Create()
    {
        var context = TestDbContextFactory.Create();
        var clientProxyMock = new Mock<IClientProxy>();

        var service = CreateService(context, clientProxyMock);

        await service.AddTransactionAsync(
            new TransactionDto(10m, "USD"));

        clientProxyMock.Verify(
            c => c.SendCoreAsync(
                "ReceiveTransaction",
                It.IsAny<object[]>(),
                default),
            Times.Once);
    }
    [Fact]
    public async Task GetAllAsync_Should_Return_Empty_When_No_Data()
    {
        var context = TestDbContextFactory.Create();
        var service = CreateService(context);

        var results = await service.GetAllAsync();

        Assert.Empty(results);
    }
    [Fact]
    public async Task AddTransaction_Should_Set_CreatedAt()
    {
        var context = TestDbContextFactory.Create();
        var service = CreateService(context);

        var result = await service.AddTransactionAsync(
            new TransactionDto(10m, "USD"));

        Assert.True(result.CreatedAt <= DateTime.UtcNow);
        Assert.True(result.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
    }
[Fact]
public async Task Concurrent_Read_And_Write_Should_Not_Throw()
{
    var context = TestDbContextFactory.Create();
    var service = CreateService(context);

    var writeTasks = Enumerable.Range(0, 200)
        .Select(_ => service.AddTransactionAsync(
            new TransactionDto(5m, "USD")));

    var readTasks = Enumerable.Range(0, 200)
        .Select(_ => service.GetAllAsync());

    var allTasks = writeTasks.Cast<Task>()
        .Concat(readTasks);

    var exception = await Record.ExceptionAsync(async () =>
        await Task.WhenAll(allTasks));

    Assert.Null(exception);
}
[Fact]
public async Task AddTransaction_Should_Not_Create_Duplicate_Ids()
{
    var context = TestDbContextFactory.Create();
    var service = CreateService(context);

    const int count = 300;

    var tasks = Enumerable.Range(0, count)
        .Select(_ => service.AddTransactionAsync(
            new TransactionDto(1m, "USD")));

    await Task.WhenAll(tasks);

    var transactions = await context.Transactions.ToListAsync();

    Assert.Equal(count, transactions.Count);
    Assert.Equal(count, transactions.Select(t => t.Id).Distinct().Count());
}
    
}
