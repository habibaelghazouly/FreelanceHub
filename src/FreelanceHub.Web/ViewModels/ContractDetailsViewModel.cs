using System.ComponentModel.DataAnnotations;
using FreelanceHub.Application.DTOs.Results;

namespace FreelanceHub.Web.ViewModels
{
	public class ContractDetailsViewModel
	{
		public ContractDetailsResult Contract { get; set; } = new();

		public SubmitReviewViewModel Review { get; set; } = new();
	}

	public class SubmitReviewViewModel
	{
		[Required]
		[Range(1, 5)]
		public int? Rating { get; set; }

		[StringLength(1000)]
		[Display(Name = "Review comment")]
		public string? Comment { get; set; }
	}
}
