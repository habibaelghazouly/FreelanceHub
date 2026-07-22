namespace FreelanceHub.Domain.Models
{
	public class Review
	{
		public int ReviewId { get; set; }

		public int ContractId { get; set; }

		public int ReviewerUserId { get; set; }

		public int RevieweeUserId { get; set; }

		public int Rating { get; set; }

		public string? Comment { get; set; }

		public DateTime CreatedAt { get; set; }

		public Contract Contract { get; set; } = null!;

		public ApplicationUser ReviewerUser { get; set; } = null!;

		public ApplicationUser RevieweeUser { get; set; } = null!;
	}
}
