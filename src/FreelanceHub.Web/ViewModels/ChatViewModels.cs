using FreelanceHub.Application.DTOs.Results;

namespace FreelanceHub.Web.ViewModels
{
	public class ChatInboxViewModel
	{
		public IReadOnlyList<ChatInboxItemResult> Applications { get; set; } = Array.Empty<ChatInboxItemResult>();
	}

	public class ChatThreadViewModel
	{
		public ChatThreadResult Thread { get; set; } = new();
	}
}
