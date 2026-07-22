using System.ComponentModel.DataAnnotations;
using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Web.ViewModels
{
	public class RegisterFreelancerViewModel : RegisterAccountViewModel
	{
		[Required]
		[StringLength(150)]
		[Display(Name = "Professional title")]
		public string ProfessionalTitle { get; set; } = string.Empty;

		[Required]
		[Range(typeof(decimal), "0.01", "9999999999999999.99")]
		[Display(Name = "Hourly rate")]
		public decimal? HourlyRate { get; set; }

		[Required]
		[StringLength(2000, MinimumLength = 20)]
		public string Bio { get; set; } = string.Empty;

		[Required]
		[EnumDataType(typeof(FreelancerExperienceLevel))]
		[Display(Name = "Experience level")]
		public FreelancerExperienceLevel? ExperienceLevel { get; set; }

		[Required]
		[EnumDataType(typeof(FreelancerAvailabilityStatus))]
		[Display(Name = "Availability status")]
		public FreelancerAvailabilityStatus? AvailabilityStatus { get; set; }

		[Url]
		[StringLength(500)]
		[Display(Name = "Portfolio URL")]
		public string? ExternalPortfolioUrl { get; set; }
	}
}
