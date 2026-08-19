using System.Net;

namespace PlaywrightApiFramework.Framework.Constants;

public static class HttpStatusCodes
{
    public static readonly int Ok = (int)HttpStatusCode.OK;
    public static readonly int Created = (int)HttpStatusCode.Created;
    public static readonly int NoContent = (int)HttpStatusCode.NoContent;
    public static readonly int BadRequest = (int)HttpStatusCode.BadRequest;
    public static readonly int Unauthorized = (int)HttpStatusCode.Unauthorized;
    public static readonly int NotFound = (int)HttpStatusCode.NotFound;
}
