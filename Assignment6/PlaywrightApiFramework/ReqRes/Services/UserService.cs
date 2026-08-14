using Microsoft.Playwright;
using PlaywrightApiFramework.Framework.API;
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
        return await Client.PostJsonAsync(UserEndpoints.Users, new
        {
            name = user.Name,
            job = user.Job
        });
    }

    public async Task<IAPIResponse> UpdateUser(int id, User user)
    {
        return await Client.PutJsonAsync(UserEndpoints.SingleUser(id), new
        {
            name = user.Name,
            job = user.Job
        });
    }

    public async Task<IAPIResponse> PatchUser(int id, object body)
    {
        return await Client.PatchJsonAsync(UserEndpoints.SingleUser(id), body);
    }

    public async Task<IAPIResponse> DeleteUser(int id)
    {
        return await Client.DeleteAsync(UserEndpoints.SingleUser(id));
    }

    public async Task<IAPIResponse> Register(object body)
    {
        return await Client.PostJsonAsync(UserEndpoints.Register, body);
    }
}
