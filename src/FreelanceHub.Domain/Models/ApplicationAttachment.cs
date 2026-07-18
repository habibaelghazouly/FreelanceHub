namespace FreelanceHub.Domain.Models
{
	public class ApplicationAttachment
	{
		public int ApplicationId { get; set; }

		// Requires the JobApplication entity (not included in this delivery).
		// public JobApplication Application { get; set; } = null!;

		public int AttachmentId { get; set; }

		public Attachment Attachment { get; set; } = null!;
	}
}