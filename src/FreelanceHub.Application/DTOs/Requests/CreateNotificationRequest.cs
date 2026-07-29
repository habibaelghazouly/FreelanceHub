using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Application.DTOs.Requests
{
	public class CreateNotificationRequest
	{
		public int RecipientUserId { get; set; }

		public int? ActorUserId { get; set; }

		public NotificationType NotificationType { get; set; }

		public string Title { get; set; } = string.Empty;

		public string Message { get; set; } = string.Empty;

		public string TargetUrl { get; set; } = string.Empty;

		public int? RelatedEntityId { get; set; }
	}
}
