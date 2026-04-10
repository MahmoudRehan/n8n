namespace n8n.Services;

public interface IAiService
{
    Task<string> GetReplyAsync(string message, CancellationToken cancellationToken = default);
}
