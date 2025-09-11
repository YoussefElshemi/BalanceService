using System.Net;

namespace Core.Exceptions;

[Serializable]
public class UnprocessableRequestException(string message) : DomainException(message)
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