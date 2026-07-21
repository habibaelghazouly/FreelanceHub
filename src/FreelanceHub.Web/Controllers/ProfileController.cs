using System.Security.Claims;
using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Exceptions;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Controllers
{
	[Authorize]
	public class ProfileController : Controller
	{
		private const long MaxProfileImageRequestSize = 2_228_224;
		private const string CompanyModalId = "edit-company-profile-modal";
		private const string FreelancerModalId = "edit-freelancer-profile-modal";

		private readonly IProfileService _profileService;

		public ProfileController(IProfileService profileService)
		{
			_profileService = profileService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			var model = await BuildProfilePageAsync(userId, HttpContext.RequestAborted);
			if (model is null)
			{
				return NotFound();
			}

			return View(model);
		}

		[HttpPost]
		[Authorize(Roles = "Client")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateCompanyProfile(
			[Bind(Prefix = nameof(ProfilePageViewModel.CompanyEditor))] EditCompanyProfileViewModel model)
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			var cancellationToken = HttpContext.RequestAborted;
			if (!ModelState.IsValid)
			{
				return await ProfileViewAsync(userId, model, null, CompanyModalId, cancellationToken);
			}

			var result = await _profileService.UpdateCompanyProfileAsync(userId, new UpdateCompanyProfileRequest
			{
				CompanyName = model.CompanyName,
				CompanyDescription = model.CompanyDescription,
				CompanyWebsite = model.CompanyWebsite
			}, cancellationToken);

			if (result.NotFound)
			{
				return NotFound();
			}

			if (!result.Succeeded)
			{
				AddErrors(result, nameof(ProfilePageViewModel.CompanyEditor));
				return await ProfileViewAsync(userId, model, null, CompanyModalId, cancellationToken);
			}

			TempData["ProfileUpdateSuccess"] = "Your company profile was updated.";
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[Authorize(Roles = "Freelancer")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateFreelancerProfile(
			[Bind(Prefix = nameof(ProfilePageViewModel.FreelancerEditor))] EditFreelancerProfileViewModel model)
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			var cancellationToken = HttpContext.RequestAborted;
			if (!ModelState.IsValid)
			{
				return await ProfileViewAsync(userId, null, model, FreelancerModalId, cancellationToken);
			}

			var result = await _profileService.UpdateFreelancerProfileAsync(userId, new UpdateFreelancerProfileRequest
			{
				ProfessionalTitle = model.ProfessionalTitle,
				HourlyRate = model.HourlyRate!.Value,
				Bio = model.Bio,
				ExperienceLevel = model.ExperienceLevel!.Value,
				AvailabilityStatus = model.AvailabilityStatus!.Value,
				ExternalPortfolioUrl = model.ExternalPortfolioUrl
			}, cancellationToken);

			if (result.NotFound)
			{
				return NotFound();
			}

			if (!result.Succeeded)
			{
				AddErrors(result, nameof(ProfilePageViewModel.FreelancerEditor));
				return await ProfileViewAsync(userId, null, model, FreelancerModalId, cancellationToken);
			}

			TempData["ProfileUpdateSuccess"] = "Your freelancer profile was updated.";
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[RequestFormLimits(MultipartBodyLengthLimit = MaxProfileImageRequestSize)]
		[RequestSizeLimit(MaxProfileImageRequestSize)]
		public async Task<IActionResult> UpdatePhoto(IFormFile? profileImage)
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
			var cancellationToken = HttpContext.RequestAborted;
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

		private async Task<IActionResult> ProfileViewAsync(
			int userId,
			EditCompanyProfileViewModel? companyEditor,
			EditFreelancerProfileViewModel? freelancerEditor,
			string openModal,
			CancellationToken cancellationToken)
		{
			var model = await BuildProfilePageAsync(
				userId,
				cancellationToken,
				companyEditor,
				freelancerEditor,
				openModal);
			if (model is not null
				&& ((openModal == CompanyModalId && model.CompanyEditor is null)
					|| (openModal == FreelancerModalId && model.FreelancerEditor is null)))
			{
				return NotFound();
			}

			return model is null ? NotFound() : View(nameof(Index), model);
		}

		private async Task<ProfilePageViewModel?> BuildProfilePageAsync(
			int userId,
			CancellationToken cancellationToken,
			EditCompanyProfileViewModel? companyEditor = null,
			EditFreelancerProfileViewModel? freelancerEditor = null,
			string? openModal = null)
		{
			var profile = await _profileService.GetByUserIdAsync(userId, cancellationToken);
			if (profile is null)
			{
				return null;
			}

			var isCompanyClient = profile.Role == "Client" && profile.ClientType == ClientType.Company;
			return new ProfilePageViewModel
			{
				Profile = profile,
				CompanyEditor = isCompanyClient
					? companyEditor ?? new EditCompanyProfileViewModel
					{
						CompanyName = profile.CompanyName ?? string.Empty,
						CompanyDescription = profile.CompanyDescription ?? string.Empty,
						CompanyWebsite = profile.CompanyWebsite
					}
					: null,
				FreelancerEditor = profile.Role == "Freelancer"
					? freelancerEditor ?? new EditFreelancerProfileViewModel
					{
						ProfessionalTitle = profile.ProfessionalTitle ?? string.Empty,
						HourlyRate = profile.HourlyRate,
						Bio = profile.Bio ?? string.Empty,
						ExperienceLevel = profile.ExperienceLevel,
						AvailabilityStatus = profile.AvailabilityStatus,
						ExternalPortfolioUrl = profile.ExternalPortfolioUrl
					}
					: null,
				OpenModal = openModal
			};
		}

		private void AddErrors(UpdateOperationResult result, string prefix)
		{
			foreach (var error in result.Errors)
			{
				var key = string.IsNullOrWhiteSpace(error.FieldName)
					? string.Empty
					: $"{prefix}.{error.FieldName}";
				ModelState.AddModelError(key, error.Message);
			}
		}
	}
}
