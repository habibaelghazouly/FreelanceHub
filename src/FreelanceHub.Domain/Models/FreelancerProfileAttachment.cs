namespace FreelanceHub.Domain.Models
{
	public class FreelancerProfileAttachment
	{
		public int FreelancerProfileId { get; set; }

		// Requires the FreelancerProfile entity (not included in this delivery).
		// public FreelancerProfile FreelancerProfile { get; set; } = null!;

		public int AttachmentId { get; set; }

		public Attachment Attachment { get; set; } = null!;

		public string AttachmentDescription { get; set; } = string.Empty;
	}
}