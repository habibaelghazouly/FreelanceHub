namespace FreelanceHub.Application.DTOs.Requests
{
	public class RegisterUserRequest
	{
		public string Username { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string Password { get; set; } = string.Empty;

		public string Role { get; set; } = string.Empty;

		public UploadedFileRequest? ProfileImage { get; set; }

		public string? CompanyName { get; set; }

		public string? CompanyDescription { get; set; }

		public string? CompanyWebsite { get; set; }

		public string? ProfessionalTitle { get; set; }

		public decimal? HourlyRate { get; set; }

		public string? Bio { get; set; }

		public string? ExperienceLevel { get; set; }

		public int? AvailabilityStatus { get; set; }

		public string? ExternalPortfolioUrl { get; set; }
	}
}
