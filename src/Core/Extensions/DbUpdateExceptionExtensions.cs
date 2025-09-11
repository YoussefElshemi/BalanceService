using Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Core.Extensions;

public static class DbUpdateExceptionExtensions
{
    public static void ThrowIfUniqueKeyViolation<T>(this DbUpdateException ex)
    {
        if (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } pgEx
            && pgEx.ConstraintName?.Contains(nameof(T), StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new IdempotencyException<T>(pgEx);
        }
    }

    public static void ThrowIfRaisedError(this DbUpdateException ex)
    {
        if (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.RaiseException } pgEx)
        {
            throw new DatabaseValidationException(pgEx);
        }
    }
}