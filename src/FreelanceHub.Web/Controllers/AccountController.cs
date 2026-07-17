using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly IApplicationUserService _applicationUserService;

		public AccountController(IApplicationUserService applicationUserService)
		{
			_applicationUserService = applicationUserService;
		}

		[HttpGet]
		public IActionResult Register(string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View(new RegisterViewModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			Stream? profileImageStream = null;
			try
			{
				profileImageStream = model.ProfileImage?.OpenReadStream();
				var result = await _applicationUserService.RegisterAsync(ToRegisterUserRequest(model, profileImageStream), HttpContext.RequestAborted);
				if (!result.Succeeded)
				{
					AddErrors(result);
					return View(model);
				}

				return RedirectToLocal(returnUrl);
			}
			finally
			{
				profileImageStream?.Dispose();
			}
		}

		[HttpGet]
		public IActionResult Login(string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View(new LoginViewModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var result = await _applicationUserService.LoginAsync(new LoginRequest
			{
				EmailOrUsername = model.EmailOrUsername,
				Password = model.Password,
				RememberMe = model.RememberMe
			});

			if (result.Succeeded)
			{
				return RedirectToLocal(returnUrl);
			}

			AddErrors(result);
			return View(model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await _applicationUserService.LogoutAsync();
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult AccessDenied()
		{
			return View();
		}

		private IActionResult RedirectToLocal(string? returnUrl)
		{
			if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
			{
				return LocalRedirect(returnUrl);
			}

			return RedirectToAction("Index", "Home");
		}

		private static RegisterUserRequest ToRegisterUserRequest(RegisterViewModel model, Stream? profileImageStream)
		{
			return new RegisterUserRequest
			{
				Username = model.Username,
				Email = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName,
				Password = model.Password,
				Role = model.Role,
				ProfileImage = model.ProfileImage is null || profileImageStream is null
					? null
					: new UploadedFileRequest(
						profileImageStream,
						model.ProfileImage.FileName,
						model.ProfileImage.ContentType,
						model.ProfileImage.Length),
				CompanyName = model.CompanyName,
				CompanyDescription = model.CompanyDescription,
				CompanyWebsite = model.CompanyWebsite,
				ProfessionalTitle = model.ProfessionalTitle,
				HourlyRate = model.HourlyRate,
				Bio = model.Bio,
				ExperienceLevel = model.ExperienceLevel,
				AvailabilityStatus = model.AvailabilityStatus,
				ExternalPortfolioUrl = model.ExternalPortfolioUrl
			};
		}

		private void AddErrors(ApplicationUserServiceResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error);
			}
		}
	}
}
