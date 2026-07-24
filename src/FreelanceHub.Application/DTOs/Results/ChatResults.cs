using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Application.DTOs.Results
{
	public class ChatInboxResult
	{
		public IReadOnlyList<ChatInboxItemResult> Applications { get; set; } = Array.Empty<ChatInboxItemResult>();
	}

	public class ChatInboxItemResult
	{
		public int ApplicationId { get; set; }

		public string JobTitle { get; set; } = string.Empty;

		public ApplicationStatus ApplicationStatus { get; set; }

		public int OtherUserId { get; set; }

		public string OtherUserDisplayName { get; set; } = string.Empty;

		public string? OtherUserProfileImageUrl { get; set; }

		public string? LatestMessage { get; set; }

		public DateTime? LatestMessageAt { get; set; }
	}

	public class ChatThreadResult
	{
		public int ApplicationId { get; set; }

		public int CurrentUserId { get; set; }

		public string JobTitle { get; set; } = string.Empty;

		public ApplicationStatus ApplicationStatus { get; set; }

		public int OtherUserId { get; set; }

		public string OtherUserDisplayName { get; set; } = string.Empty;

		public string? OtherUserProfileImageUrl { get; set; }

		public IReadOnlyList<ChatMessageResult> Messages { get; set; } = Array.Empty<ChatMessageResult>();
	}

	public class ChatMessageResult
	{
		public int ChatMessageId { get; set; }

		public int ApplicationId { get; set; }

		public int SenderUserId { get; set; }

		public string SenderDisplayName { get; set; } = string.Empty;

		public string Content { get; set; } = string.Empty;

		public DateTime SentAt { get; set; }
	}

	public class SendChatMessageResult
	{
		private SendChatMessageResult(bool succeeded, bool notFound, string? error, ChatMessageResult? message)
		{
			Succeeded = succeeded;
			NotFound = notFound;
			Error = error;
			Message = message;
		}

		public bool Succeeded { get; }

		public bool NotFound { get; }

		public string? Error { get; }

		public ChatMessageResult? Message { get; }

		public static SendChatMessageResult Success(ChatMessageResult message)
		{
			return new SendChatMessageResult(true, false, null, message);
		}

		public static SendChatMessageResult Missing()
		{
			return new SendChatMessageResult(false, true, null, null);
		}

		public static SendChatMessageResult Failed(string error)
		{
			return new SendChatMessageResult(false, false, error, null);
		}
	}
}
