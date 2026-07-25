using FreelanceHub.Application.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FreelanceHub.Web.Hubs
{
	[Authorize]
	public class ChatHub : Hub
	{
		private readonly IChatService _chatService;

		public ChatHub(IChatService chatService)
		{
			_chatService = chatService;
		}

		public async Task JoinApplication(int applicationId)
		{
			var userId = GetCurrentUserId();
			if (!await _chatService.CanAccessAsync(applicationId, userId))
			{
				throw new HubException("This application conversation was not found.");
			}

			await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(applicationId));
		}

		public async Task SendMessage(int applicationId, string content)
		{
			var result = await _chatService.SendMessageAsync(applicationId, GetCurrentUserId(), content);
			if (!result.Succeeded || result.Message is null)
			{
				throw new HubException(result.NotFound
					? "This application conversation was not found."
					: result.Error ?? "Unable to send the message.");
			}

			await Clients.Group(GetGroupName(applicationId)).SendAsync("ReceiveMessage", result.Message);
		}

		private int GetCurrentUserId()
		{
			if (!int.TryParse(Context.UserIdentifier, out var userId))
			{
				throw new HubException("Your session is no longer valid.");
			}

			return userId;
		}

		private static string GetGroupName(int applicationId)
		{
			return $"application-{applicationId}";
		}
	}
}
