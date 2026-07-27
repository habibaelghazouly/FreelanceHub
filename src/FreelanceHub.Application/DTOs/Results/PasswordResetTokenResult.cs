namespace FreelanceHub.Application.DTOs.Results
{
	public class PasswordResetTokenResult
	{
		public string Email { get; set; } = string.Empty;

		public string Token { get; set; } = string.Empty;
	}
}
