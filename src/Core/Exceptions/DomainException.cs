using System.Net;

namespace Core.Exceptions;

[Serializable]
public abstract class DomainException : Exception
{
    protected DomainException() { }
    protected DomainException(string? message) : base(message) {}
    protected DomainException(string? message, Exception innerException) : base(message, innerException) {}

    public abstract HttpStatusCode GetStatusCode();
    public abstract string GetTitle();
}