using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Core.Exceptions;

[Serializable]
public class ConcurrencyException(DbUpdateConcurrencyException dbUpdateConcurrencyException)
    : DomainException("The request failed due to a concurrency conflict. " +
                      "The resource was modified by another process.", dbUpdateConcurrencyException)
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