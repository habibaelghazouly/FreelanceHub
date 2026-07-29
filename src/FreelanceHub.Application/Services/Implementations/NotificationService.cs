using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.Repositories.Abstractions;

namespace FreelanceHub.Application.Services.Implementations
{
	public class NotificationService : INotificationService
	{
		private const int PageSize = 20;
		private const int SummarySize = 6;
		private readonly INotificationRepository _notificationRepository;
		private readonly IUnitOfWork _unitOfWork;

		public NotificationService(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
		{
			_notificationRepository = notificationRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task<Notification> CreateAsync(CreateNotificationRequest request)
		{
			var utcNow = DateTime.UtcNow;
			var groupKey = request.NotificationType == NotificationType.ChatMessage && request.RelatedEntityId.HasValue
				? GetChatGroupKey(request.RelatedEntityId.Value)
				: null;

			if (groupKey is not null)
			{
				var existing = await _notificationRepository.GetByGroupKeyAsync(request.RecipientUserId, groupKey);
				if (existing is not null)
				{
					existing.ActorUserId = request.ActorUserId;
					existing.NotificationType = request.NotificationType;
					existing.Title = Truncate(request.Title, 160);
					existing.Message = Truncate(request.Message, 500);
					existing.TargetUrl = Truncate(request.TargetUrl, 500);
					existing.RelatedEntityId = request.RelatedEntityId;
					existing.CreatedAt = utcNow;
					existing.ReadAt = null;
					return existing;
				}
			}

			var notification = new Notification
			{
				RecipientUserId = request.RecipientUserId,
				ActorUserId = request.ActorUserId,
				NotificationType = request.NotificationType,
				Title = Truncate(request.Title, 160),
				Message = Truncate(request.Message, 500),
				TargetUrl = Truncate(request.TargetUrl, 500),
				RelatedEntityId = request.RelatedEntityId,
				GroupKey = groupKey,
				CreatedAt = utcNow
			};

			await _notificationRepository.AddAsync(notification);
			return notification;
		}

		public async Task<NotificationSummaryResult> GetSummaryAsync(int userId)
		{
			var notifications = await _notificationRepository.ListLatestAsync(userId, 0, SummarySize, false);
			return new NotificationSummaryResult
			{
				UnreadCount = await _notificationRepository.CountUnreadAsync(userId),
				Notifications = notifications.Select(MapNotification).ToArray()
			};
		}

		public async Task<NotificationPageResult> GetPageAsync(int userId, bool unreadOnly, int page)
		{
			page = Math.Max(page, 1);
			var count = await _notificationRepository.CountAsync(userId, unreadOnly);
			var notifications = await _notificationRepository.ListLatestAsync(
				userId,
				(page - 1) * PageSize,
				PageSize,
				unreadOnly);

			return new NotificationPageResult
			{
				Notifications = notifications.Select(MapNotification).ToArray(),
				Page = page,
				TotalPages = (int)Math.Ceiling(count / (double)PageSize),
				UnreadOnly = unreadOnly
			};
		}

		public async Task<bool> MarkReadAsync(int notificationId, int userId)
		{
			var notification = await _notificationRepository.GetForUserAsync(notificationId, userId);
			if (notification is null || notification.ReadAt.HasValue)
			{
				return false;
			}

			notification.ReadAt = DateTime.UtcNow;
			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<bool> MarkChatReadAsync(int applicationId, int userId)
		{
			var notification = await _notificationRepository.GetByGroupKeyAsync(userId, GetChatGroupKey(applicationId));
			if (notification is null || notification.ReadAt.HasValue)
			{
				return false;
			}

			notification.ReadAt = DateTime.UtcNow;
			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<bool> MarkAllReadAsync(int userId)
		{
			var notifications = await _notificationRepository.ListUnreadAsync(userId);
			if (notifications.Count == 0)
			{
				return false;
			}

			var utcNow = DateTime.UtcNow;
			foreach (var notification in notifications)
			{
				notification.ReadAt = utcNow;
			}

			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		private static string GetChatGroupKey(int applicationId)
		{
			return $"chat:{applicationId}";
		}

		private static NotificationItemResult MapNotification(Notification notification)
		{
			return new NotificationItemResult
			{
				NotificationId = notification.NotificationId,
				NotificationType = notification.NotificationType,
				Title = notification.Title,
				Message = notification.Message,
				TargetUrl = notification.TargetUrl,
				RelatedEntityId = notification.RelatedEntityId,
				ActorDisplayName = notification.ActorUser is null ? null : GetDisplayName(notification.ActorUser),
				ActorProfileImageUrl = notification.ActorUser?.ProfileImageAttachment?.FileUrl,
				CreatedAt = notification.CreatedAt,
				IsRead = notification.ReadAt.HasValue
			};
		}

		private static string GetDisplayName(ApplicationUser user)
		{
			var fullName = $"{user.FirstName} {user.LastName}".Trim();
			return string.IsNullOrWhiteSpace(fullName) ? user.UserName ?? "User" : fullName;
		}

		private static string Truncate(string value, int maxLength)
		{
			return value.Length <= maxLength ? value : value[..maxLength];
		}
	}
}
