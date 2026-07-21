using System.ComponentModel.DataAnnotations;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Web.ViewModels
{
	public class ProfilePageViewModel
	{
		public UserProfileResult Profile { get; set; } = new();

		public EditCompanyProfileViewModel? CompanyEditor { get; set; }

		public EditFreelancerProfileViewModel? FreelancerEditor { get; set; }

		public string? OpenModal { get; set; }
	}

	public class EditCompanyProfileViewModel
	{
		[Required]
		[StringLength(150)]
		[Display(Name = "Company name")]
		public string CompanyName { get; set; } = string.Empty;

		[Required]
		[StringLength(2000)]
		[Display(Name = "Company description")]
		public string CompanyDescription { get; set; } = string.Empty;

		[Url]
		[StringLength(500)]
		[Display(Name = "Company website")]
		public string? CompanyWebsite { get; set; }
	}

	public class EditFreelancerProfileViewModel
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
