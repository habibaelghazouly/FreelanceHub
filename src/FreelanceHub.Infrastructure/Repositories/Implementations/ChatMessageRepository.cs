using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Repositories.Implementations
{
	public class ChatMessageRepository : IChatMessageRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public ChatMessageRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IReadOnlyList<Application>> ListForUserAsync(int userId)
		{
			return await _dbContext.Applications
				.AsNoTracking()
				.Where(application =>
					application.FreelancerUserId == userId
					|| application.Job.ClientUserId == userId)
				.Include(application => application.Job)
					.ThenInclude(job => job.ClientUser)
						.ThenInclude(user => user.ProfileImageAttachment)
				.Include(application => application.FreelancerUser)
					.ThenInclude(user => user.ProfileImageAttachment)
				.Include(application => application.ChatMessages
					.OrderByDescending(message => message.SentAt)
					.ThenByDescending(message => message.ChatMessageId)
					.Take(1))
				.OrderByDescending(application => application.ChatMessages
					.Select(message => (DateTime?)message.SentAt)
					.Max() ?? application.CreatedAt)
				.ToListAsync();
		}

		public Task<Application?> GetThreadForParticipantAsync(int applicationId, int userId)
		{
			return _dbContext.Applications
				.AsNoTracking()
				.Include(application => application.Job)
					.ThenInclude(job => job.ClientUser)
						.ThenInclude(user => user.ProfileImageAttachment)
				.Include(application => application.FreelancerUser)
					.ThenInclude(user => user.ProfileImageAttachment)
				.Include(application => application.ChatMessages
					.OrderBy(message => message.SentAt)
					.ThenBy(message => message.ChatMessageId))
					.ThenInclude(message => message.SenderUser)
				.SingleOrDefaultAsync(application =>
					application.ApplicationId == applicationId
					&& (application.FreelancerUserId == userId
						|| application.Job.ClientUserId == userId));
		}

		public Task<Application?> GetForParticipantAsync(int applicationId, int userId)
		{
			return _dbContext.Applications
				.AsNoTracking()
				.Include(application => application.Job)
					.ThenInclude(job => job.ClientUser)
				.Include(application => application.FreelancerUser)
				.SingleOrDefaultAsync(application =>
					application.ApplicationId == applicationId
					&& (application.FreelancerUserId == userId
						|| application.Job.ClientUserId == userId));
		}

		public Task<bool> CanAccessAsync(int applicationId, int userId)
		{
			return _dbContext.Applications
				.AsNoTracking()
				.AnyAsync(application =>
					application.ApplicationId == applicationId
					&& (application.FreelancerUserId == userId
						|| application.Job.ClientUserId == userId));
		}

		public async Task AddAsync(ChatMessage message)
		{
			await _dbContext.ChatMessages.AddAsync(message);
		}
	}
}
