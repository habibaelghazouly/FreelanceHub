using System.Security.Claims;
using FreelanceHub.Application.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Controllers
{
	[Authorize]
	public class ProfileController : Controller
	{
		private readonly IProfileService _profileService;

		public ProfileController(IProfileService profileService)
		{
			_profileService = profileService;
		}

		[HttpGet]
		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdValue, out var userId))
			{
				return Challenge();
			}

			var profile = await _profileService.GetByUserIdAsync(userId, cancellationToken);
			if (profile is null)
			{
				return NotFound();
			}

			return View(profile);
		}
	}
}
