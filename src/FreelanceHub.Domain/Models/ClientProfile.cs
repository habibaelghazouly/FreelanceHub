using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Domain.Models
{
	public class ClientProfile
	{
		public int ClientProfileId { get; set; }

		public int UserId { get; set; }

		public ApplicationUser User { get; set; } = null!;

		public ClientType ClientType { get; set; }

		public string? CompanyName { get; set; }

		public string? CompanyDescription { get; set; }

		public string? CompanyWebsite { get; set; }

		public int? CompanyLogoAttachmentId { get; set; }

		public Attachment? CompanyLogoAttachment { get; set; }

		public decimal RatingAverage { get; set; }

		public int RatingCount { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }

		public ICollection<ClientProfileAttachment> ClientProfileAttachments { get; set; } = new List<ClientProfileAttachment>();
	}
}
