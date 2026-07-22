namespace FreelanceHub.Domain.Models
{
	public class JobAttachment
	{
		public int JobId { get; set; }

		public Job Job { get; set; } = null!;

		public int AttachmentId { get; set; }

		public Attachment Attachment { get; set; } = null!;
	}
}
