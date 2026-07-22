using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FreelanceHub.Application.Services.Implementations
{
	public class ProfileService : IProfileService
	{
		private const string ClientRole = "Client";
		private const string FreelancerRole = "Freelancer";
		private const string ProfileImagesFolder = "profile-images";

		private readonly IApplicationUserRepository _applicationUserRepository;
		private readonly IAttachmentRepository _attachmentRepository;
		private readonly IFileUploadService _fileUploadService;
		private readonly IContractService _contractService;
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<ProfileService> _logger;

		public ProfileService(
			IApplicationUserRepository applicationUserRepository,
			IAttachmentRepository attachmentRepository,
			IFileUploadService fileUploadService,
			IContractService contractService,
			IUnitOfWork unitOfWork,
			UserManager<ApplicationUser> userManager,
			ILogger<ProfileService> logger)
		{
			_applicationUserRepository = applicationUserRepository;
			_attachmentRepository = attachmentRepository;
			_fileUploadService = fileUploadService;
			_contractService = contractService;
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_logger = logger;
		}

		public async Task<UserProfileResult?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
		{
			var user = await _applicationUserRepository.GetWithProfileAsync(userId, cancellationToken);
			if (user is null)
			{
				return null;
			}

			var clientProfile = user.ClientProfile;
			var freelancerProfile = user.FreelancerProfile;
			var receivedReviews = await _contractService.GetReceivedReviewsAsync(userId);
			var roles = await _userManager.GetRolesAsync(user);
			var role = roles.Contains(ClientRole, StringComparer.Ordinal)
				? ClientRole
				: roles.Contains(FreelancerRole, StringComparer.Ordinal)
					? FreelancerRole
					: roles.FirstOrDefault() ?? "Member";

			return new UserProfileResult
			{
				UserId = user.Id,
				Username = user.UserName ?? string.Empty,
				Email = user.Email ?? string.Empty,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Role = role,
				ProfileImageUrl = user.ProfileImageAttachment?.FileUrl,
				JoinedAt = user.CreatedAt,
				ClientType = clientProfile?.ClientType,
				CompanyName = clientProfile?.CompanyName,
				CompanyDescription = clientProfile?.CompanyDescription,
				CompanyWebsite = clientProfile?.CompanyWebsite,
				CompanyLogoUrl = clientProfile?.CompanyLogoAttachment?.FileUrl,
				ProfessionalTitle = freelancerProfile?.ProfessionalTitle,
				HourlyRate = freelancerProfile?.HourlyRate,
				Bio = freelancerProfile?.Bio,
				ExperienceLevel = freelancerProfile?.ExperienceLevel,
				AvailabilityStatus = freelancerProfile?.AvailabilityStatus,
				ExternalPortfolioUrl = freelancerProfile?.ExternalPortfolioUrl,
				RatingAverage = clientProfile?.RatingAverage ?? freelancerProfile?.RatingAverage ?? 0,
				RatingCount = clientProfile?.RatingCount ?? freelancerProfile?.RatingCount ?? 0,
				ReceivedReviews = receivedReviews
			};
		}

		public async Task<UpdateOperationResult> UpdateCompanyProfileAsync(
			int userId,
			UpdateCompanyProfileRequest request,
			CancellationToken cancellationToken = default)
		{
			var companyName = request.CompanyName?.Trim() ?? string.Empty;
			var companyDescription = request.CompanyDescription?.Trim() ?? string.Empty;
			var companyWebsite = NormalizeOptional(request.CompanyWebsite);
			var errors = new List<UpdateOperationError>();

			if (companyName.Length is < 1 or > 150)
			{
				errors.Add(new UpdateOperationError(nameof(request.CompanyName), "Company name is required and cannot exceed 150 characters."));
			}

			if (companyDescription.Length is < 1 or > 2000)
			{
				errors.Add(new UpdateOperationError(nameof(request.CompanyDescription), "Company description is required and cannot exceed 2000 characters."));
			}

			ValidateOptionalUrl(companyWebsite, nameof(request.CompanyWebsite), "Company website", errors);

			if (errors.Count > 0)
			{
				return UpdateOperationResult.Failed(errors.ToArray());
			}

			var user = await _applicationUserRepository.GetWithProfileForUpdateAsync(userId, cancellationToken);
			var profile = user?.ClientProfile;
			if (profile is null || profile.ClientType != ClientType.Company)
			{
				return UpdateOperationResult.Missing();
			}

			profile.CompanyName = companyName;
			profile.CompanyDescription = companyDescription;
			profile.CompanyWebsite = companyWebsite;
			profile.UpdatedAt = DateTime.UtcNow;

			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return UpdateOperationResult.Success();
		}

		public async Task<UpdateOperationResult> UpdateFreelancerProfileAsync(
			int userId,
			UpdateFreelancerProfileRequest request,
			CancellationToken cancellationToken = default)
		{
			var professionalTitle = request.ProfessionalTitle?.Trim() ?? string.Empty;
			var bio = request.Bio?.Trim() ?? string.Empty;
			var portfolioUrl = NormalizeOptional(request.ExternalPortfolioUrl);
			var errors = new List<UpdateOperationError>();

			if (professionalTitle.Length is < 1 or > 150)
			{
				errors.Add(new UpdateOperationError(nameof(request.ProfessionalTitle), "Professional title is required and cannot exceed 150 characters."));
			}

			if (request.HourlyRate < 0.01m || request.HourlyRate > 9999999999999999.99m)
			{
				errors.Add(new UpdateOperationError(nameof(request.HourlyRate), "Hourly rate must be between 0.01 and 9999999999999999.99."));
			}
			else if (decimal.Round(request.HourlyRate, 2) != request.HourlyRate)
			{
				errors.Add(new UpdateOperationError(nameof(request.HourlyRate), "Hourly rate cannot have more than two decimal places."));
			}

			if (bio.Length is < 20 or > 2000)
			{
				errors.Add(new UpdateOperationError(nameof(request.Bio), "Bio must be between 20 and 2000 characters."));
			}

			if (!Enum.IsDefined(request.ExperienceLevel))
			{
				errors.Add(new UpdateOperationError(nameof(request.ExperienceLevel), "Choose a valid experience level."));
			}

			if (!Enum.IsDefined(request.AvailabilityStatus))
			{
				errors.Add(new UpdateOperationError(nameof(request.AvailabilityStatus), "Choose a valid availability status."));
			}

			ValidateOptionalUrl(portfolioUrl, nameof(request.ExternalPortfolioUrl), "Portfolio URL", errors);

			if (errors.Count > 0)
			{
				return UpdateOperationResult.Failed(errors.ToArray());
			}

			var user = await _applicationUserRepository.GetWithProfileForUpdateAsync(userId, cancellationToken);
			var profile = user?.FreelancerProfile;
			if (profile is null)
			{
				return UpdateOperationResult.Missing();
			}

			profile.ProfessionalTitle = professionalTitle;
			profile.HourlyRate = request.HourlyRate;
			profile.Bio = bio;
			profile.ExperienceLevel = request.ExperienceLevel;
			profile.AvailabilityStatus = request.AvailabilityStatus;
			profile.ExternalPortfolioUrl = portfolioUrl;
			profile.UpdatedAt = DateTime.UtcNow;

			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return UpdateOperationResult.Success();
		}

		public async Task<bool> UpdatePhotoAsync(
			int userId,
			UploadedFileRequest profileImage,
			CancellationToken cancellationToken = default)
		{
			var user = await _userManager.FindByIdAsync(userId.ToString());
			if (user is null)
			{
				return false;
			}

			var oldAttachment = user.ProfileImageAttachmentId.HasValue
				? await _attachmentRepository.GetByIdAsync(user.ProfileImageAttachmentId.Value, cancellationToken)
				: null;
			var oldAttachmentToCleanUp = oldAttachment is not null
				&& oldAttachment.UploadedByUserId == userId
				&& oldAttachment.FileUrl.StartsWith("/uploads/profile-images/", StringComparison.OrdinalIgnoreCase)
					? oldAttachment
					: null;
			var upload = await _fileUploadService.UploadImageAsync(profileImage, ProfileImagesFolder, cancellationToken);

			try
			{
				await _unitOfWork.BeginTransactionAsync(cancellationToken);

				var attachment = new Attachment
				{
					UploadedByUserId = userId,
					OriginalFileName = upload.OriginalFileName,
					StoredFileName = upload.StoredFileName,
					FileUrl = upload.FileUrl,
					ContentType = upload.ContentType,
					FileSize = upload.FileSize
				};

				await _attachmentRepository.AddAsync(attachment, cancellationToken);
				await _unitOfWork.SaveChangesAsync(cancellationToken);

				user.ProfileImageAttachmentId = attachment.AttachmentId;
				var updateResult = await _userManager.UpdateAsync(user);
				if (!updateResult.Succeeded)
				{
					throw new InvalidOperationException(string.Join(" ", updateResult.Errors.Select(error => error.Description)));
				}

				await _unitOfWork.CommitTransactionAsync(cancellationToken);
			}
			catch
			{
				try
				{
					await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
				}
				catch (Exception rollbackException)
				{
					_logger.LogError(rollbackException, "Unable to roll back a profile photo update for user {UserId}.", userId);
				}

				await DeleteFileSafelyAsync(upload.StorageKey);
				throw;
			}

			if (oldAttachmentToCleanUp is not null)
			{
				try
				{
					var isReferenced = await _attachmentRepository.IsReferencedAsync(
						oldAttachmentToCleanUp.AttachmentId,
						CancellationToken.None);

					if (!isReferenced)
					{
						_attachmentRepository.Remove(oldAttachmentToCleanUp);
						await _unitOfWork.SaveChangesAsync(CancellationToken.None);
						await DeleteFileSafelyAsync(oldAttachmentToCleanUp.FileUrl.TrimStart('/'));
					}
				}
				catch (Exception cleanupException)
				{
					_logger.LogWarning(cleanupException, "Unable to remove old profile image attachment {AttachmentId}.", oldAttachmentToCleanUp.AttachmentId);
				}
			}

			return true;
		}

		private async Task DeleteFileSafelyAsync(string storageKey)
		{
			try
			{
				await _fileUploadService.DeleteAsync(storageKey);
			}
			catch (Exception exception)
			{
				_logger.LogWarning(exception, "Unable to delete profile image file {StorageKey}.", storageKey);
			}
		}

		private static void ValidateOptionalUrl(
			string? value,
			string fieldName,
			string displayName,
			List<UpdateOperationError> errors)
		{
			if (value is null)
			{
				return;
			}

			if (value.Length > 500
				|| !Uri.TryCreate(value, UriKind.Absolute, out var uri)
				|| (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
			{
				errors.Add(new UpdateOperationError(fieldName, $"{displayName} must be a valid HTTP or HTTPS URL of at most 500 characters."));
			}
		}

		private static string? NormalizeOptional(string? value)
		{
			return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
		}
	}
}
