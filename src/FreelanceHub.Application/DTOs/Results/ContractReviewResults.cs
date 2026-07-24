using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Application.DTOs.Results
{
	public class ContractDetailsResult
	{
		public int ContractId { get; set; }

		public int AcceptedApplicationId { get; set; }

		public int JobId { get; set; }

		public string JobTitle { get; set; } = string.Empty;

		public decimal AgreedAmount { get; set; }

		public ContractStatus ContractStatus { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime? ExpectedCompletionDate { get; set; }

		public DateTime? ActualCompletionDate { get; set; }

		public int ClientUserId { get; set; }

		public string ClientDisplayName { get; set; } = string.Empty;

		public int FreelancerUserId { get; set; }

		public string FreelancerDisplayName { get; set; } = string.Empty;

		public int RevieweeUserId { get; set; }

		public string RevieweeDisplayName { get; set; } = string.Empty;

		public bool CanSubmitReview { get; set; }

		public bool HasSubmittedReview { get; set; }
	}

	public class ReceivedReviewResult
	{
		public int ContractId { get; set; }

		public string JobTitle { get; set; } = string.Empty;

		public int ReviewerUserId { get; set; }

		public string ReviewerDisplayName { get; set; } = string.Empty;

		public string? ReviewerProfileImageUrl { get; set; }

		public int Rating { get; set; }

		public decimal ProjectPrice { get; set; }

		public IReadOnlyList<string> ProjectSkills { get; set; } = Array.Empty<string>();

		public string? Comment { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}
