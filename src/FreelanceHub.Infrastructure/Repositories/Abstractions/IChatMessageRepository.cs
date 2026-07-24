using FreelanceHub.Domain.Models;

namespace FreelanceHub.Infrastructure.Repositories.Abstractions
{
	public interface IChatMessageRepository
	{
		Task<IReadOnlyList<Application>> ListForUserAsync(int userId);

		Task<Application?> GetForParticipantAsync(int applicationId, int userId);

		Task<Application?> GetThreadForParticipantAsync(int applicationId, int userId);

		Task<bool> CanAccessAsync(int applicationId, int userId);

		Task AddAsync(ChatMessage message);
	}
}
