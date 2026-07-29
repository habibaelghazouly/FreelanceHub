using System.Security.Claims;
using FreelanceHub.Application.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Controllers
{
	[Authorize]
	[Route("notifications")]
	public class NotificationsController : Controller
	{
		private readonly INotificationService _notificationService;
		private readonly INotificationPublisher _notificationPublisher;

		public NotificationsController(
			INotificationService notificationService,
			INotificationPublisher notificationPublisher)
		{
			_notificationService = notificationService;
			_notificationPublisher = notificationPublisher;
		}

		[HttpGet("")]
		public async Task<IActionResult> Index(bool unreadOnly = false, int page = 1)
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			return View(await _notificationService.GetPageAsync(userId, unreadOnly, page));
		}

		[HttpGet("summary")]
		public async Task<IActionResult> Summary()
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			return Json(await _notificationService.GetSummaryAsync(userId));
		}

		[HttpPost("{notificationId:int}/read")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> MarkRead(int notificationId)
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			if (await _notificationService.MarkReadAsync(notificationId, userId))
			{
				await _notificationPublisher.NotifyChangedAsync(userId);
			}

			return NoContent();
		}

		[HttpPost("chat/{applicationId:int}/read")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> MarkChatRead(int applicationId)
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			if (await _notificationService.MarkChatReadAsync(applicationId, userId))
			{
				await _notificationPublisher.NotifyChangedAsync(userId);
			}

			return NoContent();
		}

		[HttpPost("read-all")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> MarkAllRead()
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			if (await _notificationService.MarkAllReadAsync(userId))
			{
				await _notificationPublisher.NotifyChangedAsync(userId);
			}

			return NoContent();
		}

		private bool TryGetCurrentUserId(out int userId)
		{
			return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
		}
	}
}
