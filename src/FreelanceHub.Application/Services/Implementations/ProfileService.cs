using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
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
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<ProfileService> _logger;

		public ProfileService(
			IApplicationUserRepository applicationUserRepository,
			IAttachmentRepository attachmentRepository,
			IFileUploadService fileUploadService,
			IUnitOfWork unitOfWork,
			UserManager<ApplicationUser> userManager,
			ILogger<ProfileService> logger)
		{
			_applicationUserRepository = applicationUserRepository;
			_attachmentRepository = attachmentRepository;
			_fileUploadService = fileUploadService;
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
				RatingCount = clientProfile?.RatingCount ?? freelancerProfile?.RatingCount ?? 0
			};
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
	}
}
