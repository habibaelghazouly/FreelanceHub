using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Domain.Models
{
	public class FreelancerProfile
	{
		public int FreelancerProfileId { get; set; }

		public int UserId { get; set; }

		public ApplicationUser User { get; set; } = null!;

		public string? ProfessionalTitle { get; set; }

		public decimal? HourlyRate { get; set; }

		public string? Bio { get; set; }

		public FreelancerExperienceLevel? ExperienceLevel { get; set; }

		public FreelancerAvailabilityStatus? AvailabilityStatus { get; set; }

		public string? ExternalPortfolioUrl { get; set; }

		public int RatingAverage { get; set; }

		public int RatingCount { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }

		public ICollection<FreelancerSkill> FreelancerSkills { get; set; } = new List<FreelancerSkill>();

		public ICollection<FreelancerProfileAttachment> FreelancerProfileAttachments { get; set; } = new List<FreelancerProfileAttachment>();
	}
}
