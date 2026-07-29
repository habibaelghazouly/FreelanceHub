using System.Security.Claims;
using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Web.ViewModels;
using FreelanceHub.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FreelanceHub.Web.Controllers
{
	public class AccountController : Controller
	{
		private const long MaxRegistrationRequestSize = 2_228_224;

		private readonly IApplicationUserService _applicationUserService;
		private readonly IEmailSender _emailSender;
		private readonly string _publicBaseUrl;

		public AccountController(
			IApplicationUserService applicationUserService,
			IEmailSender emailSender,
			IOptions<SmtpOptions> smtpOptions)
		{
			_applicationUserService = applicationUserService;
			_emailSender = emailSender;
			_publicBaseUrl = smtpOptions.Value.PublicBaseUrl;
		}

		[HttpGet]
		public IActionResult Register(string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpGet]
		public IActionResult RegisterClient(string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View(new RegisterClientViewModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[RequestFormLimits(MultipartBodyLengthLimit = MaxRegistrationRequestSize)]
		[RequestSizeLimit(MaxRegistrationRequestSize)]
		public async Task<IActionResult> RegisterClient(RegisterClientViewModel model, string? returnUrl = null)
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
				var request = PopulateAccountRequest(new RegisterClientRequest
				{
					ClientType = model.ClientType!.Value,
					CompanyName = model.CompanyName,
					CompanyDescription = model.CompanyDescription,
					CompanyWebsite = model.CompanyWebsite
				}, model, profileImageStream);
				var result = await _applicationUserService.RegisterClientAsync(request, HttpContext.RequestAborted);
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
		public IActionResult RegisterFreelancer(string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View(new RegisterFreelancerViewModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[RequestFormLimits(MultipartBodyLengthLimit = MaxRegistrationRequestSize)]
		[RequestSizeLimit(MaxRegistrationRequestSize)]
		public async Task<IActionResult> RegisterFreelancer(RegisterFreelancerViewModel model, string? returnUrl = null)
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
				var request = PopulateAccountRequest(new RegisterFreelancerRequest
				{
					ProfessionalTitle = model.ProfessionalTitle,
					HourlyRate = model.HourlyRate!.Value,
					Bio = model.Bio,
					ExperienceLevel = model.ExperienceLevel!.Value,
					AvailabilityStatus = model.AvailabilityStatus!.Value,
					ExternalPortfolioUrl = model.ExternalPortfolioUrl
				}, model, profileImageStream);
				var result = await _applicationUserService.RegisterFreelancerAsync(request, HttpContext.RequestAborted);
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
		[Authorize]
		public async Task<IActionResult> Manage()
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			return await AccountSettingsViewAsync(userId);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateAccountDetails(
			[Bind(Prefix = nameof(AccountSettingsViewModel.AccountDetails))] EditAccountDetailsViewModel model)
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			if (!ModelState.IsValid)
			{
				return await AccountSettingsViewAsync(userId, model);
			}

			var result = await _applicationUserService.UpdateAccountDetailsAsync(userId, new UpdateAccountDetailsRequest
			{
				FirstName = model.FirstName,
				LastName = model.LastName,
				Email = model.Email,
				CurrentPassword = model.CurrentPassword
			});

			if (result.NotFound)
			{
				return NotFound();
			}

			if (!result.Succeeded)
			{
				AddErrors(result, nameof(AccountSettingsViewModel.AccountDetails));
				return await AccountSettingsViewAsync(userId, model);
			}

			TempData["AccountSettingsSuccess"] = "Your account details were updated.";
			return RedirectToAction(nameof(Manage));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword(
			[Bind(Prefix = nameof(AccountSettingsViewModel.Password))] ChangePasswordViewModel model)
		{
			if (!TryGetCurrentUserId(out var userId))
			{
				return Challenge();
			}

			if (!ModelState.IsValid)
			{
				return await AccountSettingsViewAsync(userId);
			}

			var result = await _applicationUserService.ChangePasswordAsync(userId, new ChangePasswordRequest
			{
				CurrentPassword = model.CurrentPassword,
				NewPassword = model.NewPassword
			});

			if (result.NotFound)
			{
				return NotFound();
			}

			if (!result.Succeeded)
			{
				AddErrors(result, nameof(AccountSettingsViewModel.Password));
				return await AccountSettingsViewAsync(userId);
			}

			TempData["AccountSettingsSuccess"] = "Your password was changed.";
			return RedirectToAction(nameof(Manage));
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPassword()
		{
			return View(new ForgotPasswordViewModel());
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var tokenResult = await _applicationUserService.CreatePasswordResetTokenAsync(model.Email);
			if (tokenResult is not null)
			{
				var resetPath = Url.Action(
					nameof(ResetPassword),
					"Account",
					new { email = tokenResult.Email, code = tokenResult.Token });

				if (resetPath is not null && Uri.TryCreate(_publicBaseUrl, UriKind.Absolute, out var publicBaseUri))
				{
					var resetUrl = new Uri(publicBaseUri, resetPath).ToString();
					try
					{
						await _emailSender.SendAsync(
							tokenResult.Email,
							"Reset your FreelanceHub password",
							$"Use the link below to reset your password:\n\n{resetUrl}\n\nIf you did not request this, you can ignore this email.");
					}
					catch
					{
						// we just keeep it the same for any errors , so we don't leak info about email or account
					}
				}
			}

			return RedirectToAction(nameof(ForgotPasswordConfirmation));
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPassword(string? email, string? code)
		{
			if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
			{
				return RedirectToAction(nameof(ForgotPassword));
			}

			return View(new ResetPasswordViewModel { Email = email, Code = code });
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var result = await _applicationUserService.ResetPasswordAsync(new ResetPasswordRequest
			{
				Email = model.Email,
				Token = model.Code,
				NewPassword = model.NewPassword
			});

			if (result.Succeeded)
			{
				return RedirectToAction(nameof(ResetPasswordConfirmation));
			}

			foreach (var error in result.Errors)
			{
				var key = error.FieldName == nameof(ResetPasswordRequest.NewPassword)
					? nameof(ResetPasswordViewModel.NewPassword)
					: string.Empty;
				ModelState.AddModelError(key, error.Message);
			}

			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPasswordConfirmation()
		{
			return View();
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
			return RedirectToAction("Index", "Dashboard");
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

			return RedirectToAction("Index", "Dashboard");
		}

		private static TRequest PopulateAccountRequest<TRequest>(
			TRequest request,
			RegisterAccountViewModel model,
			Stream? profileImageStream)
			where TRequest : RegisterAccountRequest
		{
			request.Username = model.Username;
			request.Email = model.Email;
			request.FirstName = model.FirstName;
			request.LastName = model.LastName;
			request.Password = model.Password;
			request.ProfileImage = model.ProfileImage is null || profileImageStream is null
				? null
				: new UploadedFileRequest(
					profileImageStream,
					model.ProfileImage.FileName,
					model.ProfileImage.ContentType,
					model.ProfileImage.Length);

			return request;
		}

		private async Task<IActionResult> AccountSettingsViewAsync(
			int userId,
			EditAccountDetailsViewModel? attemptedDetails = null)
		{
			var details = await _applicationUserService.GetAccountDetailsAsync(userId);
			if (details is null)
			{
				return NotFound();
			}

			var accountDetails = attemptedDetails ?? new EditAccountDetailsViewModel
			{
				FirstName = details.FirstName,
				LastName = details.LastName,
				Email = details.Email
			};

			accountDetails.Username = details.Username;
			accountDetails.IsEmailConfirmed = details.IsEmailConfirmed
				&& string.Equals(accountDetails.Email, details.Email, StringComparison.OrdinalIgnoreCase);

			return View("Manage", new AccountSettingsViewModel
			{
				AccountDetails = accountDetails
			});
		}

		private void AddErrors(ApplicationUserServiceResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error);
			}
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

		private bool TryGetCurrentUserId(out int userId)
		{
			return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
		}
	}
}
