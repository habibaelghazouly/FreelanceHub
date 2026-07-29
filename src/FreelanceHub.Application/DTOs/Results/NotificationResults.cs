using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Application.DTOs.Results
{
	public class NotificationItemResult
	{
		public int NotificationId { get; set; }

		public NotificationType NotificationType { get; set; }

		public string Title { get; set; } = string.Empty;

		public string Message { get; set; } = string.Empty;

		public string TargetUrl { get; set; } = string.Empty;

		public int? RelatedEntityId { get; set; }

		public string? ActorDisplayName { get; set; }

		public string? ActorProfileImageUrl { get; set; }

		public DateTime CreatedAt { get; set; }

		public bool IsRead { get; set; }
	}

	public class NotificationSummaryResult
	{
		public int UnreadCount { get; set; }

		public IReadOnlyList<NotificationItemResult> Notifications { get; set; } = Array.Empty<NotificationItemResult>();
	}

	public class NotificationPageResult
	{
		public IReadOnlyList<NotificationItemResult> Notifications { get; set; } = Array.Empty<NotificationItemResult>();

		public int Page { get; set; }

		public int TotalPages { get; set; }

		public bool UnreadOnly { get; set; }
	}
}
