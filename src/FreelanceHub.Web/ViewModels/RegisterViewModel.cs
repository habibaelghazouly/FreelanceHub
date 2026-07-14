using System.ComponentModel.DataAnnotations;

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
	}
}
