using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Domain.Models;

namespace FreelanceHub.Application.Services.Abstractions
{
	public interface INotificationService
	{
		Task<Notification> CreateAsync(CreateNotificationRequest request);

		Task<NotificationSummaryResult> GetSummaryAsync(int userId);

		Task<NotificationPageResult> GetPageAsync(int userId, bool unreadOnly, int page);

		Task<bool> MarkReadAsync(int notificationId, int userId);

		Task<bool> MarkChatReadAsync(int applicationId, int userId);

		Task<bool> MarkAllReadAsync(int userId);
	}
}
