using FreelanceHub.Domain.Models;

namespace FreelanceHub.Infrastructure.Repositories.Abstractions
{
	public interface INotificationRepository
	{
		Task<IReadOnlyList<Notification>> ListLatestAsync(int userId, int skip, int take, bool unreadOnly);

		Task<int> CountAsync(int userId, bool unreadOnly);

		Task<int> CountUnreadAsync(int userId);

		Task<Notification?> GetForUserAsync(int notificationId, int userId);

		Task<Notification?> GetByGroupKeyAsync(int userId, string groupKey);

		Task<IReadOnlyList<Notification>> ListUnreadAsync(int userId);

		Task AddAsync(Notification notification);
	}
}
