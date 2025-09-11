using System.Net;
using Npgsql;

namespace Core.Exceptions;

[Serializable]
public class DatabaseValidationException(PostgresException pgEx) : DomainException(pgEx.MessageText, pgEx)
{
    public override HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.UnprocessableEntity;
    }

    public override string GetTitle()
    {
        return nameof(HttpStatusCode.UnprocessableEntity);
    }
}