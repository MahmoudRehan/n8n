using Microsoft.AspNetCore.Mvc;
using n8n.Services;
using n8n.ViewModels;

namespace n8n.Controllers;

public class ChatController(IAiService aiService, IChatService chatService) : Controller
{
    private readonly IAiService _aiService = aiService;
    private readonly IChatService _chatService = chatService;

    [HttpGet]
    public async Task<IActionResult> Index(int? conversationId, bool newConversation, CancellationToken cancellationToken)
    {
        var conversations = await _chatService.GetConversationsAsync(cancellationToken);
        var currentConversationId = newConversation ? 0 : conversationId ?? conversations.FirstOrDefault()?.Id ?? 0;

        if (currentConversationId == 0)
        {
            currentConversationId = await _chatService.CreateConversationAsync(cancellationToken: cancellationToken);
            conversations = await _chatService.GetConversationsAsync(cancellationToken);
        }

        var messages = await _chatService.GetMessagesAsync(currentConversationId, cancellationToken);

        var viewModel = new ChatIndexViewModel
        {
            CurrentConversationId = currentConversationId,
            Conversations = conversations,
            Messages = messages
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequestViewModel request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new SendMessageResponseViewModel
            {
                Success = false,
                Error = "Message cannot be empty."
            });
        }

        var conversationId = request.ConversationId;
        if (conversationId <= 0)
        {
            conversationId = await _chatService.CreateConversationAsync(cancellationToken: cancellationToken);
        }

        var userMessage = request.Message.Trim();
        await _chatService.SaveMessageAsync(conversationId, "User", userMessage, cancellationToken);
        await _chatService.TrySetConversationTitleFromMessageAsync(conversationId, userMessage, cancellationToken);

        var reply = await _aiService.GetReplyAsync(userMessage, cancellationToken);
        await _chatService.SaveMessageAsync(conversationId, "AI", reply, cancellationToken);

        return Json(new SendMessageResponseViewModel
        {
            Success = true,
            ConversationId = conversationId,
            Reply = reply
        });
    }
}
