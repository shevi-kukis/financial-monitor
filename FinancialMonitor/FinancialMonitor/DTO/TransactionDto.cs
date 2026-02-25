namespace FinancialMonitor.DTO;

public class TransactionDto
{
    public decimal Amount { get; set; }

    public string Currency { get; set; } = string.Empty;
}