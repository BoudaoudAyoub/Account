using MediatR;
using Account.Infrastructure;
using Account.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace Account.API.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(AccountDbContext accountDbContext, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest
    : IRequest<TResponse>
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger = logger ?? throw new ArgumentException(nameof(Serilog.ILogger));
    private readonly AccountDbContext _accountDbContext = accountDbContext ?? throw new ArgumentException(nameof(AccountDbContext));

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response = default!;

        string typeName = (typeof(TRequest)).Name;

        try
        {
            if (_accountDbContext.HasActiveTransaction) await next();

            IExecutionStrategy strategy = _accountDbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _accountDbContext.StartTransactionAsync();

                _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                response = await next();

                _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                await _accountDbContext.CommitTransactionAsync(transaction);
            });

            return response;
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);
            throw;
        }
    }
}