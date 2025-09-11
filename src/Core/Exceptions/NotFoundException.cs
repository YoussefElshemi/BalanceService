using System.Net;

namespace Core.Exceptions;

[Serializable]
public class NotFoundException : DomainException
{
    public override HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.NotFound;
    }

    public override string GetTitle()
    {
        return nameof(HttpStatusCode.NotFound);
    }
}