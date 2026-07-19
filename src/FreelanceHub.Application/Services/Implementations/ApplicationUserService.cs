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

		public async Task<ApplicationUserServiceResult> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
		{
			if (request.Role is not ClientRole and not FreelancerRole)
			{
				return ApplicationUserServiceResult.Failed("Choose either Client or Freelancer.");
			}

			var user = new ApplicationUser
			{
				UserName = request.Username,
				Email = request.Email,
				FirstName = request.FirstName,
				LastName = request.LastName,
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

				var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
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

				if (request.Role == ClientRole)
				{
					await _clientProfileRepository.AddAsync(new ClientProfile
					{
						UserId = user.Id,
						CompanyName = request.CompanyName,
						CompanyDescription = request.CompanyDescription,
						CompanyWebsite = request.CompanyWebsite
					}, cancellationToken);
				}
				else
				{
					await _freelancerProfileRepository.AddAsync(new FreelancerProfile
					{
						UserId = user.Id,
						ProfessionalTitle = request.ProfessionalTitle,
						HourlyRate = request.HourlyRate,
						Bio = request.Bio,
						ExperienceLevel = request.ExperienceLevel,
						AvailabilityStatus = request.AvailabilityStatus,
						ExternalPortfolioUrl = request.ExternalPortfolioUrl
					}, cancellationToken);
				}

				await _unitOfWork.SaveChangesAsync(cancellationToken);
				await _unitOfWork.CommitTransactionAsync(cancellationToken);
			}
			catch (FileUploadException ex)
			{
				await _unitOfWork.RollbackTransactionAsync(cancellationToken);

				if (profileImageUpload is not null)
				{
					await _fileUploadService.DeleteAsync(profileImageUpload.StorageKey, cancellationToken);
				}

				return ApplicationUserServiceResult.Failed(ex.Message);
			}
			catch
			{
				await _unitOfWork.RollbackTransactionAsync(cancellationToken);

				if (profileImageUpload is not null)
				{
					await _fileUploadService.DeleteAsync(profileImageUpload.StorageKey, cancellationToken);
				}

				throw;
			}

			await _signInManager.SignInAsync(user, isPersistent: false);
			return ApplicationUserServiceResult.Success();
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
