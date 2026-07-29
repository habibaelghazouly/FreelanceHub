using FreelanceHub.Application.Services.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace FreelanceHub.Web.Hubs
{
	public class SignalRNotificationPublisher : INotificationPublisher
	{
		private readonly IHubContext<NotificationHub> _hubContext;
		private readonly ILogger<SignalRNotificationPublisher> _logger;

		public SignalRNotificationPublisher(
			IHubContext<NotificationHub> hubContext,
			ILogger<SignalRNotificationPublisher> logger)
		{
			_hubContext = hubContext;
			_logger = logger;
		}

		public async Task NotifyChangedAsync(int recipientUserId)
		{
			try
			{
				await _hubContext.Clients.User(recipientUserId.ToString()).SendAsync("NotificationsChanged");
			}
			catch (Exception exception)
			{
				_logger.LogWarning(exception, "Unable to publish notifications for user {UserId}.", recipientUserId);
			}
		}
	}
}
