using Microsoft.Playwright;
using PlaywrightApiFramework.Framework.Constants;

namespace PlaywrightApiFramework.Framework.API;

public class ApiClient
{
    public IAPIRequestContext RequestContext { get; set; }

    public ApiClient(IAPIRequestContext requestContext)
    {
        RequestContext = requestContext;
    }

    public async Task<IAPIResponse> GetAsync(string url)
    {
        return await RequestContext.GetAsync(url);
    }

    public async Task<IAPIResponse> DeleteAsync(string url)
    {
        return await RequestContext.DeleteAsync(url);
    }

    public async Task DisposeAsync()
    {
        await RequestContext.DisposeAsync();
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
        return await RequestContext.FetchAsync(url, options);
    }

    APIRequestContextOptions CreateRequestOptions(string method, object body, string contentType)
    {
        var options = new APIRequestContextOptions
        {
            Method = method
        };
        if (contentType == ApiConstants.ApplicationJson)
        {
            options.Headers = GetContentTypeHeader(contentType);
            options.DataObject = body;
        }
        else if (contentType == ApiConstants.ApplicationXml)
        {
            options.Headers = GetContentTypeHeader(contentType);
            options.Data = body.ToString();
        }
        else if (contentType == ApiConstants.FormData)
        {
            var formData = RequestContext.CreateFormData();
            foreach (var field in (Dictionary<string, string>)body)
            {
                formData.Set(field.Key, field.Value);
            }
            options.Multipart = formData;
        }
        else if (contentType == ApiConstants.TextPlain)
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

    IFormData CreateFormData(Dictionary<string, string> fields)
    {
        var formData = RequestContext.CreateFormData();
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
            { ApiConstants.ContentTypeHeader, contentType }
        };
    }
}
