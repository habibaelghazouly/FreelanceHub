using System.Security.Claims;
using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.Exceptions;
using FreelanceHub.Application.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Controllers
{
	[Authorize]
	public class ProfileController : Controller
	{
		private const long MaxProfileImageRequestSize = 2_228_224;

		private readonly IProfileService _profileService;

		public ProfileController(IProfileService profileService)
		{
			_profileService = profileService;
		}

		[HttpGet]
		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			if (!TryGetCurrentUserId(out var userId))
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		[RequestFormLimits(MultipartBodyLengthLimit = MaxProfileImageRequestSize)]
		[RequestSizeLimit(MaxProfileImageRequestSize)]
		public async Task<IActionResult> UpdatePhoto(IFormFile? profileImage, CancellationToken cancellationToken)
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			if (profileImage is null || profileImage.Length == 0)
			{
				TempData["ProfilePhotoError"] = "Choose an image to upload.";
				return RedirectToAction(nameof(Index));
			}

			await using var content = profileImage.OpenReadStream();
			try
			{
				var updated = await _profileService.UpdatePhotoAsync(
					userId,
					new UploadedFileRequest(content, profileImage.FileName, profileImage.ContentType, profileImage.Length),
					cancellationToken);

				if (!updated)
				{
					return NotFound();
				}

				TempData["ProfilePhotoSuccess"] = "Your profile photo was updated.";
			}
			catch (FileUploadException exception)
			{
				TempData["ProfilePhotoError"] = exception.Message;
			}

			return RedirectToAction(nameof(Index));
		}

		private bool TryGetCurrentUserId(out int userId)
		{
			return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
		}
	}
}
