using Microsoft.Playwright;

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

    public async Task<IAPIResponse> PostJsonAsync(string url, object body)
    {
        return await RequestContext.PostAsync(url, new()
        {
            DataObject = body,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            }
        });
    }

    public async Task<IAPIResponse> PostXmlAsync(string url, string xmlBody)
    {
        return await RequestContext.PostAsync(url, new()
        {
            Data = xmlBody,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/xml" }
            }
        });
    }

    public async Task<IAPIResponse> PostFormDataAsync(string url, Dictionary<string, string> fields)
    {
        var formData = RequestContext.CreateFormData();
        foreach (var field in fields)
        {
            formData.Set(field.Key, field.Value);
        }
        return await RequestContext.PostAsync(url, new()
        {
            Multipart = formData
        });
    }

    public async Task<IAPIResponse> PostRawTextAsync(string url, string text)
    {
        return await RequestContext.PostAsync(url, new()
        {
            Data = text,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "text/plain" }
            }
        });
    }

    public async Task<IAPIResponse> PutJsonAsync(string url, object body)
    {
        return await RequestContext.PutAsync(url, new()
        {
            DataObject = body,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            }
        });
    }

    public async Task<IAPIResponse> PatchJsonAsync(string url, object body)
    {
        return await RequestContext.PatchAsync(url, new()
        {
            DataObject = body,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            }
        });
    }

    public async Task<IAPIResponse> DeleteAsync(string url)
    {
        return await RequestContext.DeleteAsync(url);
    }
}
