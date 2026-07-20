using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Application.DTOs.Requests
{
	public class RegisterFreelancerRequest : RegisterAccountRequest
	{
		public string ProfessionalTitle { get; set; } = string.Empty;

		public decimal HourlyRate { get; set; }

		public string Bio { get; set; } = string.Empty;

		public FreelancerExperienceLevel ExperienceLevel { get; set; }

		public FreelancerAvailabilityStatus AvailabilityStatus { get; set; }

		public string? ExternalPortfolioUrl { get; set; }
	}
}
