using System.Net;
using Core.Enums;

namespace Core.Exceptions;

[Serializable]
public class AccountOperationForbiddenException(AccountStatus accountStatus, AccountOperationType operation)
    : DomainException( $"{operation} is forbidden due to {nameof(AccountStatus)}: {accountStatus}")
{
    public override HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.Forbidden;
    }

    public override string GetTitle()
    {
        return nameof(HttpStatusCode.Forbidden);
    }
}