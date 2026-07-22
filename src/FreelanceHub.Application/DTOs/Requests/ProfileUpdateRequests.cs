using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Application.DTOs.Requests
{
	public class UpdateCompanyProfileRequest
	{
		public string CompanyName { get; set; } = string.Empty;

		public string CompanyDescription { get; set; } = string.Empty;

		public string? CompanyWebsite { get; set; }
	}

	public class UpdateFreelancerProfileRequest
	{
		public string ProfessionalTitle { get; set; } = string.Empty;

		public decimal HourlyRate { get; set; }

		public string Bio { get; set; } = string.Empty;

		public FreelancerExperienceLevel ExperienceLevel { get; set; }

		public FreelancerAvailabilityStatus AvailabilityStatus { get; set; }

		public string? ExternalPortfolioUrl { get; set; }
	}
}
