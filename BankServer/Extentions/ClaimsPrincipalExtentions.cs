using System.Security.Claims;

namespace BankServer.Extentions;

public static class ClaimsPrincipalExtentions
{
    private const string IdKey = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

    public static Guid GetId(this ClaimsPrincipal claimsPrincipal)
        => new(claimsPrincipal.Claims.Single(x => x.Type == IdKey).Value);
}