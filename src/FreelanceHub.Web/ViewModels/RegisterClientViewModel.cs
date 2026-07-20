using System.ComponentModel.DataAnnotations;
using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Web.ViewModels
{
	public class RegisterClientViewModel : RegisterAccountViewModel, IValidatableObject
	{
		[Required]
		[EnumDataType(typeof(ClientType))]
		[Display(Name = "Client type")]
		public ClientType? ClientType { get; set; }

		[StringLength(150)]
		[Display(Name = "Company name")]
		public string? CompanyName { get; set; }

		[StringLength(2000)]
		[Display(Name = "Company description")]
		public string? CompanyDescription { get; set; }

		[Url]
		[StringLength(500)]
		[Display(Name = "Company website")]
		public string? CompanyWebsite { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (ClientType != FreelanceHub.Domain.Enums.ClientType.Company)
			{
				yield break;
			}

			if (string.IsNullOrWhiteSpace(CompanyName))
			{
				yield return new ValidationResult(
					"Company name is required for company clients.",
					new[] { nameof(CompanyName) });
			}

			if (string.IsNullOrWhiteSpace(CompanyDescription))
			{
				yield return new ValidationResult(
					"Company description is required for company clients.",
					new[] { nameof(CompanyDescription) });
			}
		}
	}
}
