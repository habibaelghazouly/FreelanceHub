using System.ComponentModel.DataAnnotations;

namespace FreelanceHub.Web.ViewModels
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;
	}

	public class ResetPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		public string Code { get; set; } = string.Empty;

		[Required]
		[StringLength(100, MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Password)]
		[Compare(nameof(NewPassword), ErrorMessage = "The passwords do not match.")]
		[Display(Name = "Confirm new password")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
