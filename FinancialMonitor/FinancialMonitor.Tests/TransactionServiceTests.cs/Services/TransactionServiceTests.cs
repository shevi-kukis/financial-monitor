using FinancialMonitor.Data;
using FinancialMonitor.DTO;
using FinancialMonitor.Hubs;
using FinancialMonitor.Modules;
using FinancialMonitor.Services;
using FinancialMonitor.Tests.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace FinancialMonitor.Tests.Services;

public class TransactionServiceTests
{
    private TransactionService CreateService(
        AppDbContext context,
        Mock<IClientProxy>? clientProxyMock = null)
    {
        var hubContextMock =
            new Mock<IHubContext<TransactionHub>>();

        var clientsMock =
            new Mock<IHubClients>();

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
            hubContextMock.Object);
    }

    [Fact]
    public async Task AddTransaction_Should_Save_To_Database()
    {
        var context = TestDbContextFactory.Create();
        var service = CreateService(context);

        var dto = new TransactionDto
        {
            Amount = 100,
            Currency = "USD"
        };

        await service.AddTransactionAsync(dto);

        var count = await context.Transactions.CountAsync();

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task AddTransaction_Should_Assign_Valid_Status()
    {
        var context = TestDbContextFactory.Create();
        var service = CreateService(context);

        var result = await service.AddTransactionAsync(
            new TransactionDto
            {
                Amount = 50,
                Currency = "USD"
            });

        Assert.Contains(result.Status,
            new[]
            {
                TransactionStatus.Pending,
                TransactionStatus.Completed,
                TransactionStatus.Failed
            });
    }

    [Fact]
    public async Task AddTransaction_Should_Broadcast_To_Clients()
    {
        var context = TestDbContextFactory.Create();

        var clientProxyMock = new Mock<IClientProxy>();

        clientProxyMock
            .Setup(c => c.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object[]>(),
                default))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var service = CreateService(context, clientProxyMock);

        await service.AddTransactionAsync(
            new TransactionDto
            {
                Amount = 10,
                Currency = "USD"
            });

        clientProxyMock.Verify(
            c => c.SendCoreAsync(
                "ReceiveTransaction",
                It.IsAny<object[]>(),
                default),
            Times.Once);
    }

    [Fact]
    public async Task AddTransaction_Should_Handle_Concurrent_Calls_Safely()
    {
        var context = TestDbContextFactory.Create();
        var service = CreateService(context);

        var tasks = Enumerable.Range(0, 50)
            .Select(_ => service.AddTransactionAsync(
                new TransactionDto
                {
                    Amount = 20,
                    Currency = "USD"
                }));

        await Task.WhenAll(tasks);

        var count = await context.Transactions.CountAsync();

        Assert.Equal(50, count);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Transactions_Ordered_By_CreatedAt()
    {
        var context = TestDbContextFactory.Create();
        var service = CreateService(context);

        await service.AddTransactionAsync(
            new TransactionDto { Amount = 1, Currency = "USD" });

        await Task.Delay(5);

        await service.AddTransactionAsync(
            new TransactionDto { Amount = 2, Currency = "USD" });

        var result = await service.GetAllAsync();

        Assert.True(result[0].CreatedAt >= result[1].CreatedAt);
    }
}