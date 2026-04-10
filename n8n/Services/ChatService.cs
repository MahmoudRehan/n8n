using Microsoft.EntityFrameworkCore;
using n8n.Data;
using n8n.Data.Entities;
using n8n.ViewModels;

namespace n8n.Services;

public class ChatService(ApplicationDbContext dbContext) : IChatService
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<int> CreateConversationAsync(string? title = null, CancellationToken cancellationToken = default)
    {
        var conversation = new Conversation
        {
            Title = string.IsNullOrWhiteSpace(title) ? "New Chat" : title.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Conversations.Add(conversation);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return conversation.Id;
    }

    public async Task<bool> TrySetConversationTitleFromMessageAsync(int conversationId, string message, CancellationToken cancellationToken = default)
    {
        var conversation = await _dbContext.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken);
        if (conversation is null)
        {
            return false;
        }

        if (!string.Equals(conversation.Title, "New Chat", StringComparison.Ordinal))
        {
            return false;
        }

        var sanitized = message.Trim();
        if (string.IsNullOrWhiteSpace(sanitized))
        {
            return false;
        }

        conversation.Title = sanitized.Length > 60 ? sanitized[..60] + "..." : sanitized;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<ConversationListItemViewModel>> GetConversationsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Conversations
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new ConversationListItemViewModel
            {
                Id = c.Id,
                Title = c.Title,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ChatMessageViewModel>> GetMessagesAsync(int conversationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new ChatMessageViewModel
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                Sender = m.Sender,
                Content = m.Content,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ChatMessageViewModel> SaveMessageAsync(int conversationId, string sender, string content, CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            ConversationId = conversationId,
            Sender = sender,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ChatMessageViewModel
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            Sender = message.Sender,
            Content = message.Content,
            CreatedAt = message.CreatedAt
        };
    }
}
