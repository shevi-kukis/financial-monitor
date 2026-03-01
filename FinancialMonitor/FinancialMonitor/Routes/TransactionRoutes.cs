using FinancialMonitor.DTO;
using FinancialMonitor.Interfaces;
using FinancialMonitor.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FinancialMonitor.Routes;

public static class TransactionRoutes
{
    public static void MapTransactionRoutes(this IEndpointRouteBuilder app)
    {
     
        var group = app.MapGroup("/transactions");


        group.MapPost("/", CreateTransaction).WithName("CreateTransaction");
        group.MapGet("/", GetAllTransactions).WithName("GetAllTransactions");
    }

    private static async Task<Results<Ok<Transaction>, ValidationProblem>> CreateTransaction(
        [FromBody] TransactionDto dto,
        ITransactionService service,
        IValidator<TransactionDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await service.AddTransactionAsync(dto);

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<IEnumerable<Transaction>>> GetAllTransactions(
        ITransactionService service)
    {

        var result = await service.GetAllAsync();
        
        return TypedResults.Ok(result.AsEnumerable());
    }
}