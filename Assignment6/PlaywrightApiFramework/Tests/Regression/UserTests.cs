using PlaywrightApiFramework.Framework.Assertions;
using PlaywrightApiFramework.Framework.Constants;
using PlaywrightApiFramework.Framework.Reporting;
using PlaywrightApiFramework.Framework.Utilities;
using PlaywrightApiFramework.ReqRes.Endpoints;
using PlaywrightApiFramework.ReqRes.Models;
using PlaywrightApiFramework.Tests.Base;
using PlaywrightApiFramework.Tests.DataProviders;
using NUnit.Framework;

namespace PlaywrightApiFramework.Tests.Regression;

[TestFixture]
public class UserTests : ApiTestBase
{
    [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.JsonUsers))]
    public async Task CreateUser_WithJsonDataProvider_ShouldValidateBodyAndHeaders(User user)
    {
        var endpoint = UserEndpoints.Users;
        ReportHelper.PrintTest("Create user using JSON test data");
        ReportHelper.PrintValue("Request body name", user.Name);
        ReportHelper.PrintValue("Request body job", user.Job);
        var response = await UserService.CreateUser(user);
        var userResponse = await JsonHelper.Deserialize<User>(response);
        var contentType = JsonHelper.GetHeader(response, ApiConstants.ContentTypeHeader);
        ReportHelper.PrintResponse("POST " + endpoint, response);
        ReportHelper.PrintValue("Response content-type", contentType);
        ReportHelper.PrintValue("Response id", userResponse.Id);
        ApiAssert.Status(response, endpoint, HttpStatusCodes.Created);
        ApiAssert.HeaderContains(contentType, ApiConstants.ContentTypeHeader, ApiConstants.ApplicationJson);
        ApiAssert.FieldEquals(userResponse.Name, user.Name, nameof(userResponse.Name));
        ApiAssert.FieldEquals(userResponse.Job, user.Job, nameof(userResponse.Job));
        ApiAssert.FieldNotEmpty(userResponse.Id, nameof(userResponse.Id));
        ApiAssert.FieldNotEmpty(userResponse.CreatedAt, nameof(userResponse.CreatedAt));
    }

    [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CsvUsers))]
    public async Task CreateUser_WithCsvDataProvider_ShouldValidateBody(User user)
    {
        var endpoint = UserEndpoints.Users;
        ReportHelper.PrintTest("Create user using CSV test data");
        ReportHelper.PrintValue("Request body name", user.Name);
        ReportHelper.PrintValue("Request body job", user.Job);
        var response = await UserService.CreateUser(user);
        var userResponse = await JsonHelper.Deserialize<User>(response);
        ReportHelper.PrintResponse("POST " + endpoint, response);
        ApiAssert.Status(response, endpoint, HttpStatusCodes.Created);
        ApiAssert.FieldEquals(userResponse.Name, user.Name, nameof(userResponse.Name));
        ApiAssert.FieldEquals(userResponse.Job, user.Job, nameof(userResponse.Job));
    }

    [Test]
    public async Task GetUsers_PageTwo_ShouldValidateBodyAndHeaders()
    {
        var endpoint = UserEndpoints.UsersPage(TestData.PageTwo);
        ReportHelper.PrintTest("Get users page 2");
        var response = await UserService.GetUsers(TestData.PageTwo);
        var usersResponse = await JsonHelper.GetJson(response);
        var contentType = JsonHelper.GetHeader(response, ApiConstants.ContentTypeHeader);
        var users = usersResponse.GetProperty("data");
        var firstUser = users[0];
        var page = JsonHelper.GetInt(usersResponse, "page");
        var firstUserId = JsonHelper.GetInt(firstUser, "id");
        var firstUserEmail = JsonHelper.GetString(firstUser, "email");
        var firstUserFirstName = JsonHelper.GetString(firstUser, "first_name");
        ReportHelper.PrintResponse("GET " + endpoint, response);
        ReportHelper.PrintValue("Response content-type", contentType);
        ReportHelper.PrintValue("Page", page);
        ReportHelper.PrintValue("First user email", firstUserEmail);
        ApiAssert.Status(response, endpoint, HttpStatusCodes.Ok);
        ApiAssert.HeaderContains(contentType, ApiConstants.ContentTypeHeader, ApiConstants.ApplicationJson);
        ApiAssert.FieldEquals(page, TestData.PageTwo, "page");
        ApiAssert.ArrayNotEmpty(users.GetArrayLength(), "data");
        ApiAssert.GreaterThanZero(firstUserId, "id");
        ApiAssert.FieldContains(firstUserEmail, ReqResConfig.EmailDomain, "email");
        ApiAssert.FieldNotEmpty(firstUserFirstName, "first_name");
    }

    [Test]
    public async Task UpdateUser_WithPut_ShouldReturnUpdatedUser()
    {
        var endpoint = UserEndpoints.SingleUser(TestData.ExistingUserId);
        var user = TestData.UpdateUser;
        ReportHelper.PrintTest("Update user using PUT");
        ReportHelper.PrintValue("Request body name", user.Name);
        ReportHelper.PrintValue("Request body job", user.Job);
        var response = await UserService.UpdateUser(TestData.ExistingUserId, user);
        var userResponse = await JsonHelper.Deserialize<User>(response);
        ReportHelper.PrintResponse("PUT " + endpoint, response);
        ReportHelper.PrintValue("Updated at", userResponse.UpdatedAt);
        ApiAssert.Status(response, endpoint, HttpStatusCodes.Ok);
        ApiAssert.FieldEquals(userResponse.Name, user.Name, nameof(userResponse.Name));
        ApiAssert.FieldEquals(userResponse.Job, user.Job, nameof(userResponse.Job));
        ApiAssert.FieldNotEmpty(userResponse.UpdatedAt, nameof(userResponse.UpdatedAt));
    }

    [Test]
    public async Task PatchUser_ShouldReturnChangedAttribute()
    {
        var endpoint = UserEndpoints.SingleUser(TestData.ExistingUserId);
        var user = TestData.PatchUser;
        ReportHelper.PrintTest("Patch user job");
        var response = await UserService.PatchUser(TestData.ExistingUserId, user);
        var userResponse = await JsonHelper.Deserialize<User>(response);
        ReportHelper.PrintResponse("PATCH " + endpoint, response);
        ReportHelper.PrintValue("Updated job", userResponse.Job);
        ApiAssert.Status(response, endpoint, HttpStatusCodes.Ok);
        ApiAssert.FieldEquals(userResponse.Job, user.Job, nameof(userResponse.Job));
        ApiAssert.FieldNotEmpty(userResponse.UpdatedAt, nameof(userResponse.UpdatedAt));
    }

    [Test]
    public async Task DeleteUser_ShouldReturnNoContent()
    {
        var endpoint = UserEndpoints.SingleUser(TestData.ExistingUserId);
        ReportHelper.PrintTest("Delete user");
        var response = await UserService.DeleteUser(TestData.ExistingUserId);
        var body = await response.TextAsync();
        ReportHelper.PrintResponse("DELETE " + endpoint, response);
        ReportHelper.PrintValue("Response body length", body.Length);
        ApiAssert.Status(response, endpoint, HttpStatusCodes.NoContent);
        ApiAssert.EmptyBody(body, endpoint);
    }

    [Test]
    public async Task JsonContentType_ShouldCreateUser()
    {
        var endpoint = UserEndpoints.Users;
        var user = TestData.JsonContentUser;
        ReportHelper.PrintTest("JSON content type request");
        var response = await UserService.CreateUser(user, ApiConstants.ApplicationJson);
        var userResponse = await JsonHelper.Deserialize<User>(response);
        ReportHelper.PrintResponse("POST " + endpoint + " JSON", response);
        ApiAssert.Status(response, endpoint, HttpStatusCodes.Created);
        ApiAssert.FieldEquals(userResponse.Name, user.Name, nameof(userResponse.Name));
        ApiAssert.FieldEquals(userResponse.Job, user.Job, nameof(userResponse.Job));
    }

    [Test]
    public async Task XmlContentType_ShouldCreateUser()
    {
        var endpoint = UserEndpoints.Users;
        ReportHelper.PrintTest("XML content type request");
        var response = await UserService.CreateUserWithXml(TestData.XmlBody);
        var contentType = JsonHelper.GetHeader(response, ApiConstants.ContentTypeHeader);
        ReportHelper.PrintResponse("POST " + endpoint + " XML", response);
        ReportHelper.PrintValue("Response content-type", contentType);
        ApiAssert.Status(response, endpoint, HttpStatusCodes.Created);
        ApiAssert.HeaderContains(contentType, ApiConstants.ContentTypeHeader, ApiConstants.ApplicationJson);
    }

    [Test]
    public async Task FormDataContentType_ShouldCreateUser()
    {
        var endpoint = UserEndpoints.Users;
        ReportHelper.PrintTest("Form-data content type request");
        var response = await UserService.CreateUserWithFormData(TestData.FormDataUser);
        var contentType = JsonHelper.GetHeader(response, ApiConstants.ContentTypeHeader);
        ReportHelper.PrintResponse("POST " + endpoint + " form-data", response);
        ReportHelper.PrintValue("Response content-type", contentType);
        ApiAssert.Status(response, endpoint, HttpStatusCodes.Created);
        ApiAssert.HeaderContains(contentType, ApiConstants.ContentTypeHeader, ApiConstants.ApplicationJson);
    }

    [Test]
    public async Task RawTextContentType_ShouldCreateUser()
    {
        var endpoint = UserEndpoints.Users;
        ReportHelper.PrintTest("Raw text content type request");
        var response = await UserService.CreateUserWithRawText(TestData.RawTextBody);
        var contentType = JsonHelper.GetHeader(response, ApiConstants.ContentTypeHeader);
        ReportHelper.PrintResponse("POST " + endpoint + " raw text", response);
        ReportHelper.PrintValue("Response content-type", contentType);
        ApiAssert.Status(response, endpoint, HttpStatusCodes.Created);
        ApiAssert.HeaderContains(contentType, ApiConstants.ContentTypeHeader, ApiConstants.ApplicationJson);
    }

    [Test]
    public async Task DynamicDataRequest_ShouldCreateRandomUser()
    {
        var endpoint = UserEndpoints.Users;
        ReportHelper.PrintTest("Dynamic random data request");
        var user = new User
        {
            Name = DataGenerator.RandomText("student"),
            Job = DataGenerator.RandomText("job")
        };
        ReportHelper.PrintValue("Random name", user.Name);
        ReportHelper.PrintValue("Random job", user.Job);
        var response = await UserService.CreateUser(user);
        var userResponse = await JsonHelper.Deserialize<User>(response);
        ReportHelper.PrintResponse("POST " + endpoint, response);
        ApiAssert.Status(response, endpoint, HttpStatusCodes.Created);
        ApiAssert.FieldEquals(userResponse.Name, user.Name, nameof(userResponse.Name));
        ApiAssert.FieldEquals(userResponse.Job, user.Job, nameof(userResponse.Job));
    }

    [Test]
    public async Task ApiChaining_ShouldGetUserFromPreviousResponse()
    {
        var listEndpoint = UserEndpoints.UsersPage(TestData.PageTwo);
        ReportHelper.PrintTest("API chaining");
        ReportHelper.PrintStep("Call list users first");
        var listResponse = await UserService.GetUsers(TestData.PageTwo);
        var usersResponse = await JsonHelper.GetJson(listResponse);
        var chainedUserId = JsonHelper.GetInt(usersResponse.GetProperty("data")[0], "id");
        var userEndpoint = UserEndpoints.SingleUser(chainedUserId);

        ReportHelper.PrintResponse("GET " + listEndpoint, listResponse);
        ReportHelper.PrintValue("Chained user id", chainedUserId);
        ReportHelper.PrintStep("Use chained user id in second request");
        
        var userResponse = await UserService.GetUser(chainedUserId);
        var userDetails = await JsonHelper.GetJson(userResponse);
        var userDetailsId = JsonHelper.GetInt(userDetails.GetProperty("data"), "id");
        ReportHelper.PrintResponse("GET " + userEndpoint, userResponse);
        ApiAssert.Status(listResponse, listEndpoint, HttpStatusCodes.Ok);
        ApiAssert.Status(userResponse, userEndpoint, HttpStatusCodes.Ok);
        ApiAssert.FieldEquals(userDetailsId, chainedUserId, "id");
    }

    [Test]
    public async Task GetMissingUser_ShouldReturnNotFound()
    {
        var endpoint = UserEndpoints.SingleUser(TestData.MissingUserId);
        ReportHelper.PrintTest("Missing user error response");
        var response = await UserService.GetUser(TestData.MissingUserId);
        var body = await response.TextAsync();
        ReportHelper.PrintResponse("GET " + endpoint, response);
        ReportHelper.PrintValue("Response body", body.Trim());
        ApiAssert.Status(response, endpoint, HttpStatusCodes.NotFound);
        ApiAssert.FieldEquals(body.Trim(), "{}", "missing user response body");
    }

    [Test]
    public async Task RegisterWithoutPassword_ShouldReturnBadRequest()
    {
        var endpoint = UserEndpoints.Register;
        ReportHelper.PrintTest("Register without password error response");
        var response = await UserService.RegisterWithoutPassword(TestData.RegisterEmailWithoutPassword);
        var errorResponse = await JsonHelper.GetJson(response);
        var error = JsonHelper.GetString(errorResponse, "error");
        ReportHelper.PrintResponse("POST " + endpoint, response);
        ReportHelper.PrintValue("Error", error);
        ApiAssert.Status(response, endpoint, HttpStatusCodes.BadRequest);
        ApiAssert.FieldNotEmpty(error, "error");
    }

    [Test]
    public async Task CreatedUser_ShouldPersist_ButReqResDoesNotSaveData()
    {
        var createEndpoint = UserEndpoints.Users;
        var user = TestData.PersistenceUser;
        ReportHelper.PrintTest("Negative - created user persistence failure");
        ReportHelper.PrintStep("Create user first");
        var createResponse = await UserService.CreateUser(user);
        var createdUser = await JsonHelper.Deserialize<User>(createResponse);
        var createdUserId = int.Parse(createdUser.Id);
        var getEndpoint = UserEndpoints.SingleUser(createdUserId);
        ReportHelper.PrintResponse("POST " + createEndpoint, createResponse);
        ReportHelper.PrintValue("Created user id", createdUser.Id);
        ReportHelper.PrintStep("Try to fetch created user by id");
        var getResponse = await UserService.GetUser(createdUserId);
        ReportHelper.PrintResponse("GET " + getEndpoint, getResponse);
        ApiAssert.Status(getResponse, HttpStatusCodes.Ok, "Expected created ReqRes user id " + createdUser.Id + " to persist, but ReqRes test data is not saved.");
    }

    [Test]
    public async Task Requests_ShouldExecuteInParallel()
    {
        var firstEndpoint = UserEndpoints.UsersPage(TestData.PageOne);
        var secondEndpoint = UserEndpoints.SingleUser(TestData.ExistingUserId);
        var thirdEndpoint = UserEndpoints.SingleUser(TestData.SecondExistingUserId);
        ReportHelper.PrintTest("Parallel execution");
        ReportHelper.PrintStep("Create three API requests before waiting for result");
        var requests = new[]
        {
            UserService.GetUsers(TestData.PageOne),
            UserService.GetUser(TestData.ExistingUserId),
            UserService.GetUser(TestData.SecondExistingUserId)
        };
        ReportHelper.PrintStep("Await all requests together using Task.WhenAll");
        var responses = await Task.WhenAll(requests);
        ReportHelper.PrintResponse("Parallel request 1 - GET " + firstEndpoint, responses[0]);
        ReportHelper.PrintResponse("Parallel request 2 - GET " + secondEndpoint, responses[1]);
        ReportHelper.PrintResponse("Parallel request 3 - GET " + thirdEndpoint, responses[2]);
        ApiAssert.Status(responses[0], firstEndpoint, HttpStatusCodes.Ok);
        ApiAssert.Status(responses[1], secondEndpoint, HttpStatusCodes.Ok);
        ApiAssert.Status(responses[2], thirdEndpoint, HttpStatusCodes.Ok);
    }
}
