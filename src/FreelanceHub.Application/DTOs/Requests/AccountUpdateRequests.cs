namespace FreelanceHub.Application.DTOs.Requests
{
	public class UpdateAccountDetailsRequest
	{
		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string? CurrentPassword { get; set; }
	}

	public class ChangePasswordRequest
	{
		public string CurrentPassword { get; set; } = string.Empty;

		public string NewPassword { get; set; } = string.Empty;
	}
}
