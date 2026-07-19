using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace FreelanceHub.Application.Services.Implementations
{
	public class ProfileService : IProfileService
	{
		private const string ClientRole = "Client";
		private const string FreelancerRole = "Freelancer";

		private readonly IApplicationUserRepository _applicationUserRepository;
		private readonly UserManager<ApplicationUser> _userManager;

		public ProfileService(
			IApplicationUserRepository applicationUserRepository,
			UserManager<ApplicationUser> userManager)
		{
			_applicationUserRepository = applicationUserRepository;
			_userManager = userManager;
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
	}
}
