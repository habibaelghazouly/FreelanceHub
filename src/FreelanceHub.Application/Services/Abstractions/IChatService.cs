using FreelanceHub.Application.DTOs.Results;

namespace FreelanceHub.Application.Services.Abstractions
{
	public interface IChatService
	{
		Task<ChatInboxResult> GetInboxAsync(int currentUserId);

		Task<ChatThreadResult?> GetThreadAsync(int applicationId, int currentUserId);

		Task<bool> CanAccessAsync(int applicationId, int currentUserId);

		Task<SendChatMessageResult> SendMessageAsync(int applicationId, int currentUserId, string content);
	}
}
