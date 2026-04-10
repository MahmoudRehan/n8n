using n8n.ViewModels;

namespace n8n.Services;

public interface IChatService
{
    Task<int> CreateConversationAsync(string? title = null, CancellationToken cancellationToken = default);

    Task<bool> TrySetConversationTitleFromMessageAsync(int conversationId, string message, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ConversationListItemViewModel>> GetConversationsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChatMessageViewModel>> GetMessagesAsync(int conversationId, CancellationToken cancellationToken = default);

    Task<ChatMessageViewModel> SaveMessageAsync(int conversationId, string sender, string content, CancellationToken cancellationToken = default);
}
