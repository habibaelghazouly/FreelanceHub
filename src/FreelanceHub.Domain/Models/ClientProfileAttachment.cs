namespace FreelanceHub.Domain.Models
{
	public class ClientProfileAttachment
	{
		public int ClientProfileId { get; set; }

		// Requires the ClientProfile entity (not included in this delivery).
		// public ClientProfile ClientProfile { get; set; } = null!;

		public int AttachmentId { get; set; }

		public Attachment Attachment { get; set; } = null!;

		public string AttachmentType { get; set; } = string.Empty;
	}
}