using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Application.DTOs.Requests
{
	public class RegisterClientRequest : RegisterAccountRequest
	{
		public ClientType ClientType { get; set; }

		public string? CompanyName { get; set; }

		public string? CompanyDescription { get; set; }

		public string? CompanyWebsite { get; set; }
	}
}
