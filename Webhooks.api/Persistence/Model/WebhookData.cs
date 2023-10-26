﻿using System.Text.Json;

namespace Webhooks.api.Persistence.Model;

public class WebhookData
{
    public DateTime When { get; }

    public string Payload { get; }

    public string Type { get; }

    public WebhookData(WebhookType hookType, object data)
    {
        When = DateTime.UtcNow;
        Type = hookType.ToString();
        Payload = JsonSerializer.Serialize(data);
    }
}
