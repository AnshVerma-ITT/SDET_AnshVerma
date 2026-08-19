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

    public async Task<IAPIResponse> CreateUser(UserRequest user)
    {
        return await Client.PostAsync(UserEndpoints.Users, CreateUserBody(user), ContentTypes.ApplicationJson);
    }

    public async Task<IAPIResponse> CreateUser(UserRequest user, string contentType)
    {
        if (contentType == ContentTypes.ApplicationXml)
        {
            return await CreateUserWithXml($"<user><name>{user.Name}</name><job>{user.Job}</job></user>");
        }
        if (contentType == ContentTypes.FormData)
        {
            return await CreateUserWithFormData(new Dictionary<string, string>
            {
                { "name", user.Name },
                { "job", user.Job }
            });
        }
        if (contentType == ContentTypes.TextPlain)
        {
            return await CreateUserWithRawText($"name={user.Name}&job={user.Job}");
        }
        return await Client.PostAsync(UserEndpoints.Users, CreateUserBody(user), contentType);
    }

    public async Task<IAPIResponse> CreateUserWithXml(string xmlBody)
    {
        return await Client.PostAsync(UserEndpoints.Users, xmlBody, ContentTypes.ApplicationXml);
    }

    public async Task<IAPIResponse> CreateUserWithFormData(Dictionary<string, string> formData)
    {
        return await Client.PostAsync(UserEndpoints.Users, formData, ContentTypes.FormData);
    }

    public async Task<IAPIResponse> CreateUserWithRawText(string rawTextBody)
    {
        return await Client.PostAsync(UserEndpoints.Users, rawTextBody, ContentTypes.TextPlain);
    }

    public async Task<IAPIResponse> UpdateUser(int id, UserRequest user)
    {
        return await Client.PutAsync(UserEndpoints.SingleUser(id), CreateUserBody(user), ContentTypes.ApplicationJson);
    }

    public async Task<IAPIResponse> PatchUser(int id, UserRequest user)
    {
        return await Client.PatchAsync(UserEndpoints.SingleUser(id), CreateUserBody(user), ContentTypes.ApplicationJson);
    }

    public async Task<IAPIResponse> DeleteUser(int id)
    {
        return await Client.DeleteAsync(UserEndpoints.SingleUser(id));
    }

    public async Task<IAPIResponse> Register(RegisterRequest request)
    {
        return await Client.PostAsync(UserEndpoints.Register, CreateRegisterBody(request), ContentTypes.ApplicationJson);
    }

    public async Task<IAPIResponse> RegisterWithoutPassword(string email)
    {
        return await Register(new RegisterRequest
        {
            Email = email
        });
    }

    private static object CreateUserBody(UserRequest user)
    {
        return new
        {
            name = user.Name,
            job = user.Job
        };
    }

    private static object CreateRegisterBody(RegisterRequest request)
    {
        return new
        {
            email = request.Email
        };
    }
}
