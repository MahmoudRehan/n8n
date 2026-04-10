namespace n8n.ViewModels;

public class SendMessageResponseViewModel
{
    public bool Success { get; set; }

    public string Reply { get; set; } = string.Empty;

    public int ConversationId { get; set; }

    public string? Error { get; set; }
}
