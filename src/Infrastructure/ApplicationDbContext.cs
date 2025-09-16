using System.Diagnostics;
using Core.Exceptions;
using Core.Extensions;
using Core.Interfaces;
using Core.ValueObjects;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IUnitOfWork
{
    public const string DatabaseConnectionName = "BalanceServiceDb";

    public DbSet<AccountEntity> Accounts { get; init; }
    public DbSet<TransactionEntity> Transactions { get; init; }
    public DbSet<HoldEntity> Holds { get; init; }
    public DbSet<InterestAccrualEntity> InterestAccruals { get; init; }
    public DbSet<InterestProductAccountLinkEntity> InterestProductAccountLinks { get; init; }

    async Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        Activity.Current?.AddEvent(new ActivityEvent("Repository Save Requested", DateTimeOffset.UtcNow));

        try
        {
            await base.SaveChangesAsync(cancellationToken);
            Activity.Current?.AddEvent(new ActivityEvent("Repository Save Successful", DateTimeOffset.UtcNow));
        }
        catch (Exception exception)
        {
            Activity.Current?.AddEvent(new ActivityEvent("Repository Save Failed", DateTimeOffset.UtcNow));

            if (exception is DbUpdateException dbUpdateException)
            {
                dbUpdateException.ThrowIfUniqueKeyViolation<IdempotencyKey>();
                dbUpdateException.ThrowIfRaisedError();
            }

            if (exception is DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                throw new ConcurrencyException(dbUpdateConcurrencyException);
            }

            Activity.Current?.AddException(exception);
            Activity.Current?.SetStatus(ActivityStatusCode.Error, exception.Message);

            throw;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
