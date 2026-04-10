using System.Net.Http.Json;

namespace n8n.Services;

public class AiService(IHttpClientFactory httpClientFactory, ILogger<AiService> logger) : IAiService
{
    private const string WebhookUrl = "https://mahmoudrehan.app.n8n.cloud/webhook-test/test";
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<AiService> _logger = logger;

    public async Task<string> GetReplyAsync(string message, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            using var response = await client.PostAsJsonAsync(
                WebhookUrl,
                new { message },
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("AI webhook returned status code {StatusCode}", response.StatusCode);
                return "I couldn't process your request right now. Please try again.";
            }

            var aiResponse = await response.Content.ReadFromJsonAsync<AiWebhookResponse>(cancellationToken: cancellationToken);
            if (!string.IsNullOrWhiteSpace(aiResponse?.Reply))
            {
                return aiResponse.Reply;
            }

            var fallback = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(fallback))
            {
                return fallback;
            }

            return "I couldn't process your request right now. Please try again.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling AI webhook");
            return "I couldn't process your request right now. Please try again.";
        }
    }

    private sealed class AiWebhookResponse
    {
        public string Reply { get; set; } = string.Empty;
    }
}
