using FluentValidation;
using FinancialMonitor.DTO;

namespace FinancialMonitor.Validators
{
    public class TransactionValidator : AbstractValidator<TransactionDto>
    {
        public TransactionValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Currency).NotEmpty().Length(3);
        }
    }
}