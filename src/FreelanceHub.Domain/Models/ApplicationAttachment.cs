namespace FreelanceHub.Domain.Models
{
	public class ApplicationAttachment
	{
		public int ApplicationId { get; set; }

		public Application Application { get; set; } = null!;

		public int AttachmentId { get; set; }

		public Attachment Attachment { get; set; } = null!;
	}
}
