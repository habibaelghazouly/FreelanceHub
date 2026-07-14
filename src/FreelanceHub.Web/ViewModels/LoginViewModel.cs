using System.ComponentModel.DataAnnotations;

namespace FreelanceHub.Web.ViewModels
{
	public class LoginViewModel
	{
		[Required]
		[Display(Name = "Email or username")]
		public string EmailOrUsername { get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[Display(Name = "Remember me")]
		public bool RememberMe { get; set; }
	}
}
