using System.ComponentModel.DataAnnotations;

namespace FreelanceHub.Web.ViewModels
{
	public class AccountSettingsViewModel
	{
		public EditAccountDetailsViewModel AccountDetails { get; set; } = new();

		public ChangePasswordViewModel Password { get; set; } = new();
	}

	public class EditAccountDetailsViewModel
	{
		public string Username { get; set; } = string.Empty;

		[Required]
		[StringLength(100)]
		[Display(Name = "First name")]
		public string FirstName { get; set; } = string.Empty;

		[Required]
		[StringLength(100)]
		[Display(Name = "Last name")]
		public string LastName { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		[StringLength(255)]
		public string Email { get; set; } = string.Empty;

		public bool IsEmailConfirmed { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string? CurrentPassword { get; set; }
	}

	public class ChangePasswordViewModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string CurrentPassword { get; set; } = string.Empty;

		[Required]
		[StringLength(100, MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Password)]
		[Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
		[Display(Name = "Confirm new password")]
		public string ConfirmNewPassword { get; set; } = string.Empty;
	}
}
