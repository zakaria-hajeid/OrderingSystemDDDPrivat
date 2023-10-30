using System.Text;
using System.Text.Json;
using Webhooks.api.Persistence.Model;

namespace Webhooks.api.Application;

public class WebhooksSender : IWebhooksSender
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WebhooksSender(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task SendAll(IEnumerable<WebhookSubscription> receivers, WebhookData data)
    {
        var client = _httpClientFactory.CreateClient("GrantClient");
        var jsonRequest = JsonSerializer.Serialize(data);
        var tasks = receivers.Select(r => OnSendData(r, jsonRequest, client));
        await Task.WhenAll(tasks.ToArray());
    }
    private Task OnSendData(WebhookSubscription subs, string jsonData, HttpClient client)
    {
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(subs.DestUrl, UriKind.Absolute),
            Method = HttpMethod.Post,
            Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
        };

        if (!string.IsNullOrWhiteSpace(subs.Token))
        {
            request.Headers.Add("X-eshop-whtoken", subs.Token);
        }
        return client.SendAsync(request);
    }
}
