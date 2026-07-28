using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Repositories.Implementations
{
	public class NotificationRepository : INotificationRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public NotificationRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IReadOnlyList<Notification>> ListLatestAsync(int userId, int skip, int take, bool unreadOnly)
		{
			var query = _dbContext.Notifications
				.AsNoTracking()
				.Where(notification => notification.RecipientUserId == userId);

			if (unreadOnly)
			{
				query = query.Where(notification => notification.ReadAt == null);
			}

			return await query
				.Include(notification => notification.ActorUser)
					.ThenInclude(user => user!.ProfileImageAttachment)
				.OrderByDescending(notification => notification.CreatedAt)
				.ThenByDescending(notification => notification.NotificationId)
				.Skip(skip)
				.Take(take)
				.ToListAsync();
		}

		public Task<int> CountAsync(int userId, bool unreadOnly)
		{
			return _dbContext.Notifications.CountAsync(notification =>
				notification.RecipientUserId == userId
				&& (!unreadOnly || notification.ReadAt == null));
		}

		public Task<int> CountUnreadAsync(int userId)
		{
			return _dbContext.Notifications.CountAsync(notification =>
				notification.RecipientUserId == userId
				&& notification.ReadAt == null);
		}

		public Task<Notification?> GetForUserAsync(int notificationId, int userId)
		{
			return _dbContext.Notifications.SingleOrDefaultAsync(notification =>
				notification.NotificationId == notificationId
				&& notification.RecipientUserId == userId);
		}

		public Task<Notification?> GetByGroupKeyAsync(int userId, string groupKey)
		{
			return _dbContext.Notifications.SingleOrDefaultAsync(notification =>
				notification.RecipientUserId == userId
				&& notification.GroupKey == groupKey);
		}

		public async Task<IReadOnlyList<Notification>> ListUnreadAsync(int userId)
		{
			return await _dbContext.Notifications
				.Where(notification =>
					notification.RecipientUserId == userId
					&& notification.ReadAt == null)
				.ToListAsync();
		}

		public async Task AddAsync(Notification notification)
		{
			await _dbContext.Notifications.AddAsync(notification);
		}
	}
}
