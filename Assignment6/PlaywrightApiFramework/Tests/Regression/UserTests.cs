using PlaywrightApiFramework.Framework.Fixtures;
using PlaywrightApiFramework.Framework.Reporting;
using PlaywrightApiFramework.Framework.TestData;
using PlaywrightApiFramework.Framework.Utilities;
using PlaywrightApiFramework.ReqRes.Models;
using PlaywrightApiFramework.ReqRes.Services;
using NUnit.Framework;

namespace PlaywrightApiFramework.Tests.Regression;

[TestFixture]
public class UserTests
{
    public ApiFixture Fixture { get; set; }
    public UserService UserService { get; set; }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Fixture = new ApiFixture();
        await Fixture.StartAsync();
        UserService = new UserService(Fixture.Client);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await Fixture.StopAsync();
    }

    public static List<User> JsonUsers()
    {
        return TestDataHelper.ReadJsonList<User>("ReqRes/TestData/users.json");
    }

    public static List<User> CsvUsers()
    {
        var rows = TestDataHelper.ReadCsv("ReqRes/TestData/users.csv");
        var users = new List<User>();
        foreach (var row in rows)
        {
            users.Add(new User
            {
                Name = row["name"],
                Job = row["job"]
            });
        }
        return users;
    }

    [TestCaseSource(nameof(JsonUsers))]
    public async Task CreateUser_WithJsonDataProvider_ShouldValidateBodyAndHeaders(User user)
    {
        ReportHelper.PrintTest("Create user using JSON test data");
        ReportHelper.PrintValue("Request body name", user.Name);
        ReportHelper.PrintValue("Request body job", user.Job);
        var response = await UserService.CreateUser(user);
        var json = await JsonHelper.GetJson(response);
        var contentType = JsonHelper.GetHeader(response, "content-type");
        ReportHelper.PrintResponse("POST /api/users", response);
        ReportHelper.PrintValue("Response content-type", contentType);
        ReportHelper.PrintValue("Response id", JsonHelper.GetString(json, "id"));
        Assert.That(response.Status, Is.EqualTo(201));
        Assert.That(contentType, Does.Contain("application/json"));
        Assert.That(JsonHelper.GetString(json, "name"), Is.EqualTo(user.Name));
        Assert.That(JsonHelper.GetString(json, "job"), Is.EqualTo(user.Job));
        Assert.That(JsonHelper.GetString(json, "id"), Is.Not.Empty);
        Assert.That(JsonHelper.GetString(json, "createdAt"), Is.Not.Empty);
    }

    [TestCaseSource(nameof(CsvUsers))]
    public async Task CreateUser_WithCsvDataProvider_ShouldValidateBody(User user)
    {
        ReportHelper.PrintTest("Create user using CSV test data");
        ReportHelper.PrintValue("Request body name", user.Name);
        ReportHelper.PrintValue("Request body job", user.Job);
        var response = await UserService.CreateUser(user);
        var json = await JsonHelper.GetJson(response);
        ReportHelper.PrintResponse("POST /api/users", response);
        Assert.That(response.Status, Is.EqualTo(201));
        Assert.That(JsonHelper.GetString(json, "name"), Is.EqualTo(user.Name));
        Assert.That(JsonHelper.GetString(json, "job"), Is.EqualTo(user.Job));
    }

    [Test]
    public async Task GetUsers_PageTwo_ShouldValidateBodyAndHeaders()
    {
        ReportHelper.PrintTest("Get users page 2");
        var response = await UserService.GetUsers(2);
        var json = await JsonHelper.GetJson(response);
        var contentType = JsonHelper.GetHeader(response, "content-type");
        var firstUser = json.GetProperty("data")[0];
        ReportHelper.PrintResponse("GET /api/users?page=2", response);
        ReportHelper.PrintValue("Response content-type", contentType);
        ReportHelper.PrintValue("Page", JsonHelper.GetInt(json, "page"));
        ReportHelper.PrintValue("First user email", firstUser.GetProperty("email").GetString() ?? "");
        Assert.That(response.Status, Is.EqualTo(200));
        Assert.That(contentType, Does.Contain("application/json"));
        Assert.That(JsonHelper.GetInt(json, "page"), Is.EqualTo(2));
        Assert.That(json.GetProperty("data").GetArrayLength(), Is.GreaterThan(0));
        Assert.That(firstUser.GetProperty("id").GetInt32(), Is.GreaterThan(0));
        Assert.That(firstUser.GetProperty("email").GetString(), Does.Contain("@reqres.in"));
        Assert.That(firstUser.GetProperty("first_name").GetString(), Is.Not.Empty);
    }

    [Test]
    public async Task UpdateUser_WithPut_ShouldReturnUpdatedUser()
    {
        ReportHelper.PrintTest("Update user using PUT");
        var user = new User
        {
            Name = "Rahul Updated",
            Job = "Automation Tester"
        };
        ReportHelper.PrintValue("Request body name", user.Name);
        ReportHelper.PrintValue("Request body job", user.Job);
        var response = await UserService.UpdateUser(2, user);
        var json = await JsonHelper.GetJson(response);
        ReportHelper.PrintResponse("PUT /api/users/2", response);
        ReportHelper.PrintValue("Updated at", JsonHelper.GetString(json, "updatedAt"));
        Assert.That(response.Status, Is.EqualTo(200));
        Assert.That(JsonHelper.GetString(json, "name"), Is.EqualTo(user.Name));
        Assert.That(JsonHelper.GetString(json, "job"), Is.EqualTo(user.Job));
        Assert.That(JsonHelper.GetString(json, "updatedAt"), Is.Not.Empty);
    }

    [Test]
    public async Task PatchUser_ShouldReturnChangedAttribute()
    {
        ReportHelper.PrintTest("Patch user job");
        var body = new
        {
            job = "Lead QA"
        };
        var response = await UserService.PatchUser(2, body);
        var json = await JsonHelper.GetJson(response);
        ReportHelper.PrintResponse("PATCH /api/users/2", response);
        ReportHelper.PrintValue("Updated job", JsonHelper.GetString(json, "job"));
        Assert.That(response.Status, Is.EqualTo(200));
        Assert.That(JsonHelper.GetString(json, "job"), Is.EqualTo("Lead QA"));
        Assert.That(JsonHelper.GetString(json, "updatedAt"), Is.Not.Empty);
    }

    [Test]
    public async Task DeleteUser_ShouldReturnNoContent()
    {
        ReportHelper.PrintTest("Delete user");
        var response = await UserService.DeleteUser(2);
        var body = await response.TextAsync();
        ReportHelper.PrintResponse("DELETE /api/users/2", response);
        ReportHelper.PrintValue("Response body length", body.Length);
        Assert.That(response.Status, Is.EqualTo(204));
        Assert.That(body, Is.EqualTo(""));
    }

    [Test]
    public async Task JsonContentType_ShouldCreateUser()
    {
        ReportHelper.PrintTest("JSON content type request");
        var user = new
        {
            name = "Json User",
            job = "Tester"
        };
        var response = await Fixture.Client.PostJsonAsync("/api/users", user);
        var json = await JsonHelper.GetJson(response);
        ReportHelper.PrintResponse("POST /api/users JSON", response);
        Assert.That(response.Status, Is.EqualTo(201));
        Assert.That(JsonHelper.GetString(json, "name"), Is.EqualTo("Json User"));
        Assert.That(JsonHelper.GetString(json, "job"), Is.EqualTo("Tester"));
    }

    [Test]
    public async Task XmlContentType_ShouldCreateUser()
    {
        ReportHelper.PrintTest("XML content type request");
        var xml = "<user><name>Xml User</name><job>Tester</job></user>";
        var response = await Fixture.Client.PostXmlAsync("/api/users", xml);
        var contentType = JsonHelper.GetHeader(response, "content-type");
        ReportHelper.PrintResponse("POST /api/users XML", response);
        ReportHelper.PrintValue("Response content-type", contentType);
        Assert.That(response.Status, Is.EqualTo(201));
        Assert.That(contentType, Does.Contain("application/json"));
    }

    [Test]
    public async Task FormDataContentType_ShouldCreateUser()
    {
        ReportHelper.PrintTest("Form-data content type request");
        var fields = new Dictionary<string, string>
        {
            { "name", "Form User" },
            { "job", "Tester" }
        };
        var response = await Fixture.Client.PostFormDataAsync("/api/users", fields);
        var contentType = JsonHelper.GetHeader(response, "content-type");
        ReportHelper.PrintResponse("POST /api/users form-data", response);
        ReportHelper.PrintValue("Response content-type", contentType);
        Assert.That(response.Status, Is.EqualTo(201));
        Assert.That(contentType, Does.Contain("application/json"));
    }

    [Test]
    public async Task RawTextContentType_ShouldCreateUser()
    {
        ReportHelper.PrintTest("Raw text content type request");
        var response = await Fixture.Client.PostRawTextAsync("/api/users", "name=Raw User&job=Tester");
        var contentType = JsonHelper.GetHeader(response, "content-type");
        ReportHelper.PrintResponse("POST /api/users raw text", response);
        ReportHelper.PrintValue("Response content-type", contentType);
        Assert.That(response.Status, Is.EqualTo(201));
        Assert.That(contentType, Does.Contain("application/json"));
    }

    [Test]
    public async Task DynamicDataRequest_ShouldCreateRandomUser()
    {
        ReportHelper.PrintTest("Dynamic random data request");
        var user = DataGenerator.CreateUser();
        ReportHelper.PrintValue("Random name", user.Name);
        ReportHelper.PrintValue("Random job", user.Job);
        var response = await UserService.CreateUser(user);
        var json = await JsonHelper.GetJson(response);
        ReportHelper.PrintResponse("POST /api/users", response);
        Assert.That(response.Status, Is.EqualTo(201));
        Assert.That(JsonHelper.GetString(json, "name"), Is.EqualTo(user.Name));
        Assert.That(JsonHelper.GetString(json, "job"), Is.EqualTo(user.Job));
    }

    [Test]
    public async Task ApiChaining_ShouldGetUserFromPreviousResponse()
    {
        ReportHelper.PrintTest("API chaining");
        ReportHelper.PrintStep("Call list users first");
        var listResponse = await UserService.GetUsers(2);
        var listJson = await JsonHelper.GetJson(listResponse);
        var chainedUserId = listJson.GetProperty("data")[0].GetProperty("id").GetInt32();
        ReportHelper.PrintResponse("GET /api/users?page=2", listResponse);
        ReportHelper.PrintValue("Chained user id", chainedUserId);
        ReportHelper.PrintStep("Use chained user id in second request");
        var userResponse = await UserService.GetUser(chainedUserId);
        var userJson = await JsonHelper.GetJson(userResponse);
        ReportHelper.PrintResponse("GET /api/users/" + chainedUserId, userResponse);
        Assert.That(listResponse.Status, Is.EqualTo(200));
        Assert.That(userResponse.Status, Is.EqualTo(200));
        Assert.That(userJson.GetProperty("data").GetProperty("id").GetInt32(), Is.EqualTo(chainedUserId));
    }

    [Test]
    public async Task Requests_ShouldExecuteInParallel()
    {
        ReportHelper.PrintTest("Parallel execution");
        ReportHelper.PrintStep("Create three API requests before waiting for result");
        var requests = new[]
        {
            UserService.GetUsers(1),
            UserService.GetUser(2),
            UserService.GetUser(3)
        };
        ReportHelper.PrintStep("Await all requests together using Task.WhenAll");
        var responses = await Task.WhenAll(requests);
        ReportHelper.PrintResponse("Parallel request 1 - GET /api/users?page=1", responses[0]);
        ReportHelper.PrintResponse("Parallel request 2 - GET /api/users/2", responses[1]);
        ReportHelper.PrintResponse("Parallel request 3 - GET /api/users/3", responses[2]);
        Assert.That(responses[0].Status, Is.EqualTo(200));
        Assert.That(responses[1].Status, Is.EqualTo(200));
        Assert.That(responses[2].Status, Is.EqualTo(200));
    }
}
