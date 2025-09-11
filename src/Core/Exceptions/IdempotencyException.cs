using System.Net;
using Npgsql;

namespace Core.Exceptions;

[Serializable]
public class IdempotencyException<T>(PostgresException pgEx) : DomainException($"{typeof(T)} must be unique", pgEx)
{
    public override HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.Conflict;
    }

    public override string GetTitle()
    {
        return nameof(HttpStatusCode.Conflict);
    }
}