namespace FreelanceHub.Domain.Models.Attachments
{
	public class ContractAttachment
	{
		public int ContractId { get; set; }

		// Requires the Contract entity (not included in this delivery).
		// public Contract Contract { get; set; } = null!;

		public int AttachmentId { get; set; }

		public Attachment Attachment { get; set; } = null!;
	}
}