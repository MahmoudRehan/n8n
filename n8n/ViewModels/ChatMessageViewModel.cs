namespace n8n.ViewModels;

public class ChatMessageViewModel
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public string Sender { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
