using Microsoft.Playwright;
using PlaywrightApiFramework.Framework.Constants;

namespace PlaywrightApiFramework.Framework.API;

public class ApiClient
{
    private readonly IAPIRequestContext requestContext;

    public ApiClient(IAPIRequestContext requestContext)
    {
        this.requestContext = requestContext;
    }

    public async Task<IAPIResponse> GetAsync(string url)
    {
        return await requestContext.GetAsync(url);
    }

    public async Task<IAPIResponse> DeleteAsync(string url)
    {
        return await requestContext.DeleteAsync(url);
    }

    public async Task DisposeAsync()
    {
        await requestContext.DisposeAsync();
    }

    public async Task<IAPIResponse> PostAsync(string url, object body, string contentType)
    {
        return await SendAsync("POST", url, body, contentType);
    }

    public async Task<IAPIResponse> PutAsync(string url, object body, string contentType)
    {
        return await SendAsync("PUT", url, body, contentType);
    }

    public async Task<IAPIResponse> PatchAsync(string url, object body, string contentType)
    {
        return await SendAsync("PATCH", url, body, contentType);
    }

    async Task<IAPIResponse> SendAsync(string method, string url, object body, string contentType)
    {
        var options = CreateRequestOptions(method, body, contentType);
        return await requestContext.FetchAsync(url, options);
    }

    APIRequestContextFetchOptions CreateRequestOptions(string method, object body, string contentType)
    {
        var options = new APIRequestContextFetchOptions
        {
            Method = method
        };
        if (contentType == ContentTypes.ApplicationJson)
        {
            options.Headers = GetContentTypeHeader(contentType);
            options.DataObject = body;
        }
        else if (contentType == ContentTypes.ApplicationXml)
        {
            options.Headers = GetContentTypeHeader(contentType);
            options.Data = body.ToString();
        }
        else if (contentType == ContentTypes.FormData)
        {
            options.Multipart = CreateFormData((Dictionary<string, string>)body);
        }
        else if (contentType == ContentTypes.TextPlain)
        {
            options.Headers = GetContentTypeHeader(contentType);
            options.Data = body.ToString();
        }
        else
        {
            options.Headers = GetContentTypeHeader(contentType);
            options.Data = body.ToString();
        }
        return options;
    }

    FormData CreateFormData(Dictionary<string, string> fields)
    {
        var formData = requestContext.CreateFormData();
        foreach (var field in fields)
        {
            formData.Set(field.Key, field.Value);
        }
        return formData;
    }

    Dictionary<string, string> GetContentTypeHeader(string contentType)
    {
        return new Dictionary<string, string>
        {
            { ContentTypes.ContentTypeHeader, contentType }
        };
    }
}
