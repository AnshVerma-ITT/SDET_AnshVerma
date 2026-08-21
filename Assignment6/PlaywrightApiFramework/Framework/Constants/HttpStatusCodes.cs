using System.Net;

namespace PlaywrightApiFramework.Framework.Constants;

public static class HttpStatusCodes
{
    public static readonly int Ok = ToInt(HttpStatusCode.OK);
    public static readonly int Created = ToInt(HttpStatusCode.Created);
    public static readonly int NoContent = ToInt(HttpStatusCode.NoContent);
    public static readonly int BadRequest = ToInt(HttpStatusCode.BadRequest);
    public static readonly int Unauthorized = ToInt(HttpStatusCode.Unauthorized);
    public static readonly int NotFound = ToInt(HttpStatusCode.NotFound);

    static int ToInt(HttpStatusCode statusCode)
    {
        return (int)statusCode;
    }
}
