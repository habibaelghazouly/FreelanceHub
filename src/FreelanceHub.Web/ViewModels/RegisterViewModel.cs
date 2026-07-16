using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FreelanceHub.Web.ViewModels
{
	public class RegisterViewModel
	{
		[Required]
		[StringLength(50, MinimumLength = 3)]
		public string Username { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		[StringLength(255)]
		public string Email { get; set; } = string.Empty;

		[Required]
		[StringLength(100)]
		[Display(Name = "First name")]
		public string FirstName { get; set; } = string.Empty;

		[Required]
		[StringLength(100)]
		[Display(Name = "Last name")]
		public string LastName { get; set; } = string.Empty;

		[Required]
		[StringLength(100, MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Password)]
		[Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
		[Display(Name = "Confirm password")]
		public string ConfirmPassword { get; set; } = string.Empty;

		[Required]
		[RegularExpression("Client|Freelancer", ErrorMessage = "Choose either Client or Freelancer.")]
		public string Role { get; set; } = "Client";

		[Display(Name = "Profile image")]
		public IFormFile? ProfileImage { get; set; }

		[StringLength(150)]
		[Display(Name = "Company name")]
		public string? CompanyName { get; set; }

		[Display(Name = "Company description")]
		public string? CompanyDescription { get; set; }

		[StringLength(500)]
		[Display(Name = "Company website")]
		public string? CompanyWebsite { get; set; }

		[StringLength(150)]
		[Display(Name = "Professional title")]
		public string? ProfessionalTitle { get; set; }

		[Range(0, 9999999999999999.99)]
		[Display(Name = "Hourly rate")]
		public decimal? HourlyRate { get; set; }

		public string? Bio { get; set; }

		[StringLength(30)]
		[Display(Name = "Experience level")]
		public string? ExperienceLevel { get; set; }

		[Display(Name = "Availability status")]
		public int? AvailabilityStatus { get; set; }

		[StringLength(500)]
		[Display(Name = "Portfolio URL")]
		public string? ExternalPortfolioUrl { get; set; }
	}
}
