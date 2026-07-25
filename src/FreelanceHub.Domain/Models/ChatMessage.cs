namespace FreelanceHub.Domain.Models
{
	public class ChatMessage
	{
		public int ChatMessageId { get; set; }

		public int ApplicationId { get; set; }

		public int SenderUserId { get; set; }

		public string Content { get; set; } = string.Empty;

		public DateTime SentAt { get; set; }

		public Application Application { get; set; } = null!;

		public ApplicationUser SenderUser { get; set; } = null!;
	}
}
