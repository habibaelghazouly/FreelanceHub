using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Application.DTOs.Results
{
	public class UserProfileResult
	{
		public int UserId { get; set; }

		public string Username { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string Role { get; set; } = "Member";

		public string? ProfileImageUrl { get; set; }

		public DateTime JoinedAt { get; set; }

		public ClientType? ClientType { get; set; }

		public string? CompanyName { get; set; }

		public string? CompanyDescription { get; set; }

		public string? CompanyWebsite { get; set; }

		public string? CompanyLogoUrl { get; set; }

		public string? ProfessionalTitle { get; set; }

		public decimal? HourlyRate { get; set; }

		public string? Bio { get; set; }

		public FreelancerExperienceLevel? ExperienceLevel { get; set; }

		public FreelancerAvailabilityStatus? AvailabilityStatus { get; set; }

		public string? ExternalPortfolioUrl { get; set; }

		public decimal RatingAverage { get; set; }

		public int RatingCount { get; set; }

		public IReadOnlyList<ReceivedReviewResult> ReceivedReviews { get; set; } = Array.Empty<ReceivedReviewResult>();
	}
}
