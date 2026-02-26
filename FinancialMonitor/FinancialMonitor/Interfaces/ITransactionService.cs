using FinancialMonitor.DTO;
using FinancialMonitor.Models;

namespace FinancialMonitor.Interfaces;

public interface ITransactionService
{
    Task<Transaction> AddTransactionAsync(TransactionDto dto);
    Task<List<Transaction>> GetAllAsync();
}