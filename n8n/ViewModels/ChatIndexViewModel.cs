namespace n8n.ViewModels;

public class ChatIndexViewModel
{
    public int CurrentConversationId { get; set; }

    public IReadOnlyList<ConversationListItemViewModel> Conversations { get; set; } = [];

    public IReadOnlyList<ChatMessageViewModel> Messages { get; set; } = [];
}
