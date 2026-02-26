using FinancialMonitor.DTO;
using FinancialMonitor.Interfaces;
using FinancialMonitor.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FinancialMonitor.Routes;

public static class TransactionRoutes
{
    public static void MapTransactionRoutes(this IEndpointRouteBuilder app)
    {
     
        app.MapPost("/transactions", async Task<Results<Ok<FinancialMonitor.Models.Transaction>, ValidationProblem>> (
    [FromBody] TransactionDto dto,
    [FromServices] ITransactionService service,
    [FromServices] IValidator<TransactionDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            var result = await service.AddTransactionAsync(dto);

          
            return TypedResults.Ok(result);
        })
    .WithName("CreateTransaction");

  
        app.MapGet("/transactions", async Task<Ok<IEnumerable<Transaction>>> (
            [FromServices] ITransactionService service) =>
        {
            var result = await service.GetAllAsync();
            return TypedResults.Ok(result as IEnumerable<Transaction>);
        })
        .WithName("GetAllTransactions");
    }
}