using FinancialMonitor.DTO;
using FinancialMonitor.Interfaces;
using FinancialMonitor.Modules;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FinancialMonitor.Routes;

public static class TransactionRoutes
{
    public static void MapTransactionRoutes(this IEndpointRouteBuilder app)
    {
        // POST Endpoint
        app.MapPost("/transactions", async Task<Results<Ok<FinancialMonitor.Modules.Transaction>, ValidationProblem>> (
    [FromBody] TransactionDto dto,
    [FromServices] ITransactionService service,
    [FromServices] IValidator<TransactionDto> validator) =>
        {
            // 1. וולידציה על ה-DTO הקיים (Amount ו-Currency)
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            // 2. שליחה ל-Service. 
            // ה-Service יקבל את ה-DTO, ייצר ID, סטטוס וזמן, ויחזיר לנו אובייקט Transaction מלא.
            var result = await service.AddTransactionAsync(dto);

            // 3. החזרת האובייקט המלא שנוצר
            return TypedResults.Ok(result);
        })
    .WithName("CreateTransaction");

        // GET Endpoint
        app.MapGet("/transactions", async Task<Ok<IEnumerable<Transaction>>> (
            [FromServices] ITransactionService service) =>
        {
            var result = await service.GetAllAsync();
            return TypedResults.Ok(result as IEnumerable<Transaction>);
        })
        .WithName("GetAllTransactions");
    }
}