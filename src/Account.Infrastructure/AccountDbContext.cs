using Account.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Account.Domain.Aggregates.UserAggregate;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Account.Infrastructure.EntityTypeConfigurations.UserConfigurations;
namespace Account.Infrastructure;
public class AccountDbContext(DbContextOptions<AccountDbContext> options) 
    : IdentityDbContext(options), 
      IUnitOfWork
{
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    private IDbContextTransaction _currentTransaction = default!;
    public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;
    public bool HasActiveTransaction => _currentTransaction is not null;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApplicationUserEntityTypeConfiguration());
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        //TODO:add events domain dispatch

        // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
        // performed through the DbContext will be committed
        return await base.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<IDbContextTransaction> StartTransactionAsync()
    {
        // If the given dbContextTransaction is not null then return the default value of "IDbContextTransaction" which is 'null'
        if (_currentTransaction is not null) return default!;

        // Initialize a new instance to the current transaction using Database.BeginTransactionAsync() EFC method
        _currentTransaction = await Database.BeginTransactionAsync();

        // Return the current transaction
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction, nameof(transaction));

        if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            DisposeTransaction();
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            DisposeTransaction();
        }
    }

    private void DisposeTransaction()
    {
        if (_currentTransaction is null) return;
        _currentTransaction.Dispose();
        _currentTransaction = default!;
    }
}