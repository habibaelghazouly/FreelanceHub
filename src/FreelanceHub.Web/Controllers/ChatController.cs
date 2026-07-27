using System.Security.Claims;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Controllers
{
	[Authorize(Roles = "Client,Freelancer")]
	[Route("messages")]
	public class ChatController : Controller
	{
		private readonly IChatService _chatService;

		public ChatController(IChatService chatService)
		{
			_chatService = chatService;
		}

		[HttpGet("")]
		public async Task<IActionResult> Inbox()
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			var inbox = await _chatService.GetInboxAsync(userId);
			return View(new ChatInboxViewModel { Applications = inbox.Applications });
		}

		[HttpGet("application/{applicationId:int}")]
		public async Task<IActionResult> Thread(int applicationId)
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			var thread = await _chatService.GetThreadAsync(applicationId, userId);
			return thread is null ? NotFound() : View(new ChatThreadViewModel { Thread = thread });
		}

		private bool TryGetCurrentUserId(out int userId)
		{
			return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
		}
	}
}
