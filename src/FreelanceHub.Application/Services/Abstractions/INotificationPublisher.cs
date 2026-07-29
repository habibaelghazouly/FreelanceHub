namespace FreelanceHub.Application.Services.Abstractions
{
	public interface INotificationPublisher
	{
		Task NotifyChangedAsync(int recipientUserId);
	}
}
