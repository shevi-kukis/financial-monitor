using FinancialMonitor.Data;
using FinancialMonitor.DTO;
using FinancialMonitor.Hubs;
using FinancialMonitor.Interfaces;
using FinancialMonitor.Modules;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FinancialMonitor.Services;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;
    private readonly IHubContext<TransactionHub> _hubContext;
    private readonly IServiceScopeFactory _scopeFactory;

    public TransactionService(
        AppDbContext context,
        IHubContext<TransactionHub> hubContext,
        IServiceScopeFactory scopeFactory)
    {
        _context = context;
        _hubContext = hubContext;
        _scopeFactory = scopeFactory;
    }

    public async Task<Transaction> AddTransactionAsync(TransactionDto dto)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = dto.Amount,
            Currency = dto.Currency,
            Status = TransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveTransaction", transaction);

        _ = ProcessTransactionAsync(transaction.Id);

        return transaction;
    }

    public async Task<List<Transaction>> GetAllAsync()
    {
        return await _context.Transactions
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    private async Task ProcessTransactionAsync(Guid transactionId)
    {
        await Task.Delay(3000); 

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var transaction = await context.Transactions.FindAsync(transactionId);

        if (transaction == null)
            return;

        var random = new Random();

        transaction.Status =
            random.Next(0, 2) == 0
                ? TransactionStatus.Completed
                : TransactionStatus.Failed;

        await context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveTransaction", transaction);
    }
}