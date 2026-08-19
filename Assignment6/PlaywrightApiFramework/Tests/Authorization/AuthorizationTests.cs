using PlaywrightApiFramework.Framework.Assertions;
using PlaywrightApiFramework.Framework.Constants;
using PlaywrightApiFramework.Framework.Reporting;
using PlaywrightApiFramework.Framework.Utilities;
using PlaywrightApiFramework.ReqRes.Endpoints;
using PlaywrightApiFramework.Tests.Base;
using NUnit.Framework;

namespace PlaywrightApiFramework.Tests.Authorization;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AuthorizationTests : ApiTestBase
{
    [Test]
    public async Task MissingApiKey_ShouldReturnUnauthorized()
    {
        var endpoint = UserEndpoints.SingleUser(TestData.ExistingUserId);
        ReportHelper.PrintTest("Authorization - missing API key");
        var clientWithoutAuth = await Fixture.CreateClientWithoutAuthAsync();
        try
        {
            var response = await clientWithoutAuth.GetAsync(endpoint);
            var errorResponse = await JsonHelper.GetJson(response);
            var error = JsonHelper.GetString(errorResponse, "error");
            ReportHelper.PrintResponse("GET " + endpoint + " without auth header", response);
            ReportHelper.PrintValue("Error", error);
            ApiAssert.Status(response, endpoint, HttpStatusCodes.Unauthorized);
            ApiAssert.FieldNotEmpty(error, "error");
        }
        finally
        {
            await clientWithoutAuth.DisposeAsync();
        }
    }
}
