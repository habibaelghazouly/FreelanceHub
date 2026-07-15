namespace FreelanceHub.Domain.Models.Attachments
{
	public class JobAttachment
	{
		public int JobId { get; set; }

		// Requires the Job entity (not included in this delivery).
		// public Job Job { get; set; } = null!;

		public int AttachmentId { get; set; }

		public Attachment Attachment { get; set; } = null!;
	}
}