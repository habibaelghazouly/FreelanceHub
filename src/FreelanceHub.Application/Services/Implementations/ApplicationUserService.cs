using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Exceptions;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace FreelanceHub.Application.Services.Implementations
{
	public class ApplicationUserService : IApplicationUserService
	{
		private const string ClientRole = "Client";
		private const string FreelancerRole = "Freelancer";
		private const string ProfileImagesFolder = "profile-images";

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IAttachmentRepository _attachmentRepository;
		private readonly IClientProfileRepository _clientProfileRepository;
		private readonly IFreelancerProfileRepository _freelancerProfileRepository;
		private readonly IFileUploadService _fileUploadService;

		public ApplicationUserService(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IUnitOfWork unitOfWork,
			IAttachmentRepository attachmentRepository,
			IClientProfileRepository clientProfileRepository,
			IFreelancerProfileRepository freelancerProfileRepository,
			IFileUploadService fileUploadService)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_unitOfWork = unitOfWork;
			_attachmentRepository = attachmentRepository;
			_clientProfileRepository = clientProfileRepository;
			_freelancerProfileRepository = freelancerProfileRepository;
			_fileUploadService = fileUploadService;
		}

		public Task<ApplicationUserServiceResult> RegisterClientAsync(RegisterClientRequest request, CancellationToken cancellationToken = default)
		{
			var errors = ValidateAccount(request);

			if (!Enum.IsDefined(request.ClientType))
			{
				errors.Add("Choose either Individual or Company.");
			}

			if (request.ClientType == ClientType.Company)
			{
				if (string.IsNullOrWhiteSpace(request.CompanyName))
				{
					errors.Add("Company name is required for company clients.");
				}
				else if (request.CompanyName.Trim().Length > 150)
				{
					errors.Add("Company name cannot exceed 150 characters.");
				}

				if (string.IsNullOrWhiteSpace(request.CompanyDescription))
				{
					errors.Add("Company description is required for company clients.");
				}
				else if (request.CompanyDescription.Trim().Length > 2000)
				{
					errors.Add("Company description cannot exceed 2000 characters.");
				}

				ValidateOptionalUrl(request.CompanyWebsite, "Company website", errors);
			}

			if (errors.Count > 0)
			{
				return Task.FromResult(ApplicationUserServiceResult.Failed(errors.ToArray()));
			}

			var isCompany = request.ClientType == ClientType.Company;
			return RegisterAsync(
				request,
				ClientRole,
				(userId, token) => _clientProfileRepository.AddAsync(new ClientProfile
				{
					UserId = userId,
					ClientType = request.ClientType,
					CompanyName = isCompany ? request.CompanyName!.Trim() : null,
					CompanyDescription = isCompany ? request.CompanyDescription!.Trim() : null,
					CompanyWebsite = isCompany ? NormalizeOptional(request.CompanyWebsite) : null
				}, token),
				cancellationToken);
		}

		public Task<ApplicationUserServiceResult> RegisterFreelancerAsync(RegisterFreelancerRequest request, CancellationToken cancellationToken = default)
		{
			var errors = ValidateAccount(request);

			if (string.IsNullOrWhiteSpace(request.ProfessionalTitle))
			{
				errors.Add("Professional title is required.");
			}
			else if (request.ProfessionalTitle.Trim().Length > 150)
			{
				errors.Add("Professional title cannot exceed 150 characters.");
			}

			if (request.HourlyRate < 0.01m || request.HourlyRate > 9999999999999999.99m)
			{
				errors.Add("Hourly rate must be between 0.01 and 9999999999999999.99.");
			}
			else if (decimal.Round(request.HourlyRate, 2) != request.HourlyRate)
			{
				errors.Add("Hourly rate cannot have more than two decimal places.");
			}

			var normalizedBio = request.Bio?.Trim() ?? string.Empty;
			var bioLength = normalizedBio.Length;
			if (bioLength is < 20 or > 2000)
			{
				errors.Add("Bio must be between 20 and 2000 characters.");
			}

			if (!Enum.IsDefined(request.ExperienceLevel))
			{
				errors.Add("Choose a valid experience level.");
			}

			if (!Enum.IsDefined(request.AvailabilityStatus))
			{
				errors.Add("Choose a valid availability status.");
			}

			ValidateOptionalUrl(request.ExternalPortfolioUrl, "Portfolio URL", errors);

			if (errors.Count > 0)
			{
				return Task.FromResult(ApplicationUserServiceResult.Failed(errors.ToArray()));
			}

			return RegisterAsync(
				request,
				FreelancerRole,
				(userId, token) => _freelancerProfileRepository.AddAsync(new FreelancerProfile
				{
					UserId = userId,
					ProfessionalTitle = request.ProfessionalTitle.Trim(),
					HourlyRate = request.HourlyRate,
					Bio = normalizedBio,
					ExperienceLevel = request.ExperienceLevel,
					AvailabilityStatus = request.AvailabilityStatus,
					ExternalPortfolioUrl = NormalizeOptional(request.ExternalPortfolioUrl)
				}, token),
				cancellationToken);
		}

		private async Task<ApplicationUserServiceResult> RegisterAsync(
			RegisterAccountRequest request,
			string role,
			Func<int, CancellationToken, Task> addProfileAsync,
			CancellationToken cancellationToken)
		{
			var user = new ApplicationUser
			{
				UserName = request.Username.Trim(),
				Email = request.Email.Trim(),
				FirstName = request.FirstName.Trim(),
				LastName = request.LastName.Trim(),
				UserStatus = UserStatus.Active
			};

			await _unitOfWork.BeginTransactionAsync(cancellationToken);
			FileUploadResult? profileImageUpload = null;

			try
			{
				var result = await _userManager.CreateAsync(user, request.Password);
				if (!result.Succeeded)
				{
					await _unitOfWork.RollbackTransactionAsync(cancellationToken);
					return ApplicationUserServiceResult.Failed(result.Errors);
				}

				var roleResult = await _userManager.AddToRoleAsync(user, role);
				if (!roleResult.Succeeded)
				{
					await _unitOfWork.RollbackTransactionAsync(cancellationToken);
					return ApplicationUserServiceResult.Failed(roleResult.Errors);
				}

				if (request.ProfileImage is not null)
				{
					profileImageUpload = await _fileUploadService.UploadImageAsync(request.ProfileImage, ProfileImagesFolder, cancellationToken);
					var attachment = new Attachment
					{
						UploadedByUserId = user.Id,
						OriginalFileName = profileImageUpload.OriginalFileName,
						StoredFileName = profileImageUpload.StoredFileName,
						FileUrl = profileImageUpload.FileUrl,
						ContentType = profileImageUpload.ContentType,
						FileSize = profileImageUpload.FileSize
					};

					await _attachmentRepository.AddAsync(attachment, cancellationToken);
					await _unitOfWork.SaveChangesAsync(cancellationToken);

					user.ProfileImageAttachmentId = attachment.AttachmentId;
					var updateResult = await _userManager.UpdateAsync(user);
					if (!updateResult.Succeeded)
					{
						await _unitOfWork.RollbackTransactionAsync(cancellationToken);
						await _fileUploadService.DeleteAsync(profileImageUpload.StorageKey, cancellationToken);
						return ApplicationUserServiceResult.Failed(updateResult.Errors);
					}
				}

				await addProfileAsync(user.Id, cancellationToken);

				await _unitOfWork.SaveChangesAsync(cancellationToken);
				await _unitOfWork.CommitTransactionAsync(cancellationToken);
			}
			catch (FileUploadException ex)
			{
				try
				{
					await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
				}
				finally
				{
					if (profileImageUpload is not null)
					{
						await _fileUploadService.DeleteAsync(profileImageUpload.StorageKey, CancellationToken.None);
					}
				}

				return ApplicationUserServiceResult.Failed(ex.Message);
			}
			catch
			{
				try
				{
					await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
				}
				finally
				{
					if (profileImageUpload is not null)
					{
						await _fileUploadService.DeleteAsync(profileImageUpload.StorageKey, CancellationToken.None);
					}
				}

				throw;
			}

			await _signInManager.SignInAsync(user, isPersistent: false);
			return ApplicationUserServiceResult.Success();
		}

		private static List<string> ValidateAccount(RegisterAccountRequest request)
		{
			var errors = new List<string>();

			var usernameLength = request.Username?.Trim().Length ?? 0;
			if (usernameLength is < 3 or > 50)
			{
				errors.Add("Username must be between 3 and 50 characters.");
			}

			if (string.IsNullOrWhiteSpace(request.Email) || request.Email.Trim().Length > 255)
			{
				errors.Add("Email is required and cannot exceed 255 characters.");
			}

			if (string.IsNullOrWhiteSpace(request.FirstName) || request.FirstName.Trim().Length > 100)
			{
				errors.Add("First name is required and cannot exceed 100 characters.");
			}

			if (string.IsNullOrWhiteSpace(request.LastName) || request.LastName.Trim().Length > 100)
			{
				errors.Add("Last name is required and cannot exceed 100 characters.");
			}

			if (string.IsNullOrWhiteSpace(request.Password))
			{
				errors.Add("Password is required.");
			}

			return errors;
		}

		private static void ValidateOptionalUrl(string? value, string fieldName, List<string> errors)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return;
			}

			if (value.Trim().Length > 500
				|| !Uri.TryCreate(value.Trim(), UriKind.Absolute, out var uri)
				|| (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
			{
				errors.Add($"{fieldName} must be a valid HTTP or HTTPS URL of at most 500 characters.");
			}
		}

		private static string? NormalizeOptional(string? value)
		{
			return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
		}

		public async Task<ApplicationUserServiceResult> LoginAsync(LoginRequest request)
		{
			var user = await _userManager.FindByEmailAsync(request.EmailOrUsername)
				?? await _userManager.FindByNameAsync(request.EmailOrUsername);

			if (user is null)
			{
				return ApplicationUserServiceResult.Failed("Invalid login attempt.");
			}

			var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, lockoutOnFailure: true);
			if (result.Succeeded)
			{
				return ApplicationUserServiceResult.Success();
			}

			if (result.IsLockedOut)
			{
				return ApplicationUserServiceResult.LockedOut("This account is temporarily locked. Try again later.");
			}

			return ApplicationUserServiceResult.Failed("Invalid login attempt.");
		}

		public Task LogoutAsync()
		{
			return _signInManager.SignOutAsync();
		}
	}
}
