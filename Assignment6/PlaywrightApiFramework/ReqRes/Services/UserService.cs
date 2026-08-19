using Microsoft.Playwright;
using PlaywrightApiFramework.Framework.API;
using PlaywrightApiFramework.Framework.Constants;
using PlaywrightApiFramework.ReqRes.Endpoints;
using PlaywrightApiFramework.ReqRes.Models;

namespace PlaywrightApiFramework.ReqRes.Services;

public class UserService
{
    public ApiClient Client { get; set; }

    public UserService(ApiClient client)
    {
        Client = client;
    }

    public async Task<IAPIResponse> GetUsers(int page)
    {
        return await Client.GetAsync(UserEndpoints.UsersPage(page));
    }

    public async Task<IAPIResponse> GetUser(int id)
    {
        return await Client.GetAsync(UserEndpoints.SingleUser(id));
    }

    public async Task<IAPIResponse> CreateUser(User user)
    {
        return await Client.PostAsync(UserEndpoints.Users, CreateUserBody(user), ApiConstants.ApplicationJson);
    }

    public async Task<IAPIResponse> CreateUser(User user, string contentType)
    {
        return await Client.PostAsync(UserEndpoints.Users, CreateUserBody(user), contentType);
    }

    public async Task<IAPIResponse> CreateUserWithXml(string xmlBody)
    {
        return await Client.PostAsync(UserEndpoints.Users, xmlBody, ApiConstants.ApplicationXml);
    }

    public async Task<IAPIResponse> CreateUserWithFormData(Dictionary<string, string> formData)
    {
        return await Client.PostAsync(UserEndpoints.Users, formData, ApiConstants.FormData);
    }

    public async Task<IAPIResponse> CreateUserWithRawText(string rawTextBody)
    {
        return await Client.PostAsync(UserEndpoints.Users, rawTextBody, ApiConstants.TextPlain);
    }

    public async Task<IAPIResponse> UpdateUser(int id, User user)
    {
        return await Client.PutAsync(UserEndpoints.SingleUser(id), CreateUserBody(user), ApiConstants.ApplicationJson);
    }

    public async Task<IAPIResponse> PatchUser(int id, User user)
    {
        return await Client.PatchAsync(UserEndpoints.SingleUser(id), CreateUserBody(user), ApiConstants.ApplicationJson);
    }

    public async Task<IAPIResponse> DeleteUser(int id)
    {
        return await Client.DeleteAsync(UserEndpoints.SingleUser(id));
    }

    public async Task<IAPIResponse> Register(object body)
    {
        return await Client.PostAsync(UserEndpoints.Register, body, ApiConstants.ApplicationJson);
    }

    public async Task<IAPIResponse> RegisterWithoutPassword(string email)
    {
        return await Register(new
        {
            email
        });
    }

    object CreateUserBody(User user)
    {
        return new
        {
            name = user.Name,
            job = user.Job
        };
    }
}
