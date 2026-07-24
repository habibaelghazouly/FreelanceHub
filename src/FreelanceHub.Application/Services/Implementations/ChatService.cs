using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using ApplicationEntity = FreelanceHub.Domain.Models.Application;

namespace FreelanceHub.Application.Services.Implementations
{
	public class ChatService : IChatService
	{
		private const int MaxMessageLength = 2000;
		private readonly IChatMessageRepository _chatMessageRepository;
		private readonly IUnitOfWork _unitOfWork;

		public ChatService(IChatMessageRepository chatMessageRepository, IUnitOfWork unitOfWork)
		{
			_chatMessageRepository = chatMessageRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task<ChatInboxResult> GetInboxAsync(int currentUserId)
		{
			var applications = await _chatMessageRepository.ListForUserAsync(currentUserId);

			return new ChatInboxResult
			{
				Applications = applications.Select(application =>
				{
					var otherUser = GetOtherUser(application, currentUserId);
					var latestMessage = application.ChatMessages.SingleOrDefault();

					return new ChatInboxItemResult
					{
						ApplicationId = application.ApplicationId,
						JobTitle = application.Job.Title,
						ApplicationStatus = application.ApplicationStatus,
						OtherUserId = otherUser.Id,
						OtherUserDisplayName = GetDisplayName(otherUser),
						OtherUserProfileImageUrl = otherUser.ProfileImageAttachment?.FileUrl,
						LatestMessage = latestMessage?.Content,
						LatestMessageAt = latestMessage?.SentAt
					};
				}).ToArray()
			};
		}

		public async Task<ChatThreadResult?> GetThreadAsync(int applicationId, int currentUserId)
		{
			var application = await _chatMessageRepository.GetThreadForParticipantAsync(applicationId, currentUserId);
			if (application is null)
			{
				return null;
			}

			var otherUser = GetOtherUser(application, currentUserId);
			return new ChatThreadResult
			{
				ApplicationId = application.ApplicationId,
				CurrentUserId = currentUserId,
				JobTitle = application.Job.Title,
				ApplicationStatus = application.ApplicationStatus,
				OtherUserId = otherUser.Id,
				OtherUserDisplayName = GetDisplayName(otherUser),
				OtherUserProfileImageUrl = otherUser.ProfileImageAttachment?.FileUrl,
				Messages = application.ChatMessages.Select(MapMessage).ToArray()
			};
		}

		public Task<bool> CanAccessAsync(int applicationId, int currentUserId)
		{
			return _chatMessageRepository.CanAccessAsync(applicationId, currentUserId);
		}

		public async Task<SendChatMessageResult> SendMessageAsync(int applicationId, int currentUserId, string content)
		{
			var normalizedContent = content?.Trim() ?? string.Empty;
			if (normalizedContent.Length is < 1 or > MaxMessageLength)
			{
				return SendChatMessageResult.Failed($"Message must be between 1 and {MaxMessageLength} characters.");
			}

			var application = await _chatMessageRepository.GetForParticipantAsync(applicationId, currentUserId);
			if (application is null)
			{
				return SendChatMessageResult.Missing();
			}

			var sender = currentUserId == application.FreelancerUserId
				? application.FreelancerUser
				: application.Job.ClientUser;
			var message = new ChatMessage
			{
				ApplicationId = applicationId,
				SenderUserId = currentUserId,
				Content = normalizedContent,
				SentAt = DateTime.UtcNow
			};

			await _chatMessageRepository.AddAsync(message);

			try
			{
				await _unitOfWork.SaveChangesAsync();
				return SendChatMessageResult.Success(new ChatMessageResult
				{
					ChatMessageId = message.ChatMessageId,
					ApplicationId = applicationId,
					SenderUserId = currentUserId,
					SenderDisplayName = GetDisplayName(sender),
					Content = normalizedContent,
					SentAt = message.SentAt
				});
			}
			catch (DbUpdateException)
			{
				return SendChatMessageResult.Failed("Unable to send the message. Please try again.");
			}
		}

		private static ChatMessageResult MapMessage(ChatMessage message)
		{
			return new ChatMessageResult
			{
				ChatMessageId = message.ChatMessageId,
				ApplicationId = message.ApplicationId,
				SenderUserId = message.SenderUserId,
				SenderDisplayName = GetDisplayName(message.SenderUser),
				Content = message.Content,
				SentAt = message.SentAt
			};
		}

		private static ApplicationUser GetOtherUser(ApplicationEntity application, int currentUserId)
		{
			return currentUserId == application.FreelancerUserId
				? application.Job.ClientUser
				: application.FreelancerUser;
		}

		private static string GetDisplayName(ApplicationUser user)
		{
			var fullName = $"{user.FirstName} {user.LastName}".Trim();
			return string.IsNullOrWhiteSpace(fullName) ? user.UserName ?? "User" : fullName;
		}
	}
}
