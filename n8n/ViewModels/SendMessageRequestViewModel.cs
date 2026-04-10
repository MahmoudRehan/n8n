using System.ComponentModel.DataAnnotations;

namespace n8n.ViewModels;

public class SendMessageRequestViewModel
{
    public int ConversationId { get; set; }

    [Required]
    [StringLength(4000)]
    public string Message { get; set; } = string.Empty;
}
