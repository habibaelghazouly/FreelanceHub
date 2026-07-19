namespace FreelanceHub.Domain.Models
{
	public class ContractAttachment
	{
		public int ContractId { get; set; }

		public Contract Contract { get; set; } = null!;

		public int AttachmentId { get; set; }

		public Attachment Attachment { get; set; } = null!;
	}
}
