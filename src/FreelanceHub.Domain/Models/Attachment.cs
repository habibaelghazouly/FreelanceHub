namespace FreelanceHub.Domain.Models
{
	public class Attachment
	{
		public int AttachmentId { get; set; }

		public int UploadedByUserId { get; set; }

		public ApplicationUser UploadedByUser { get; set; } = null!;

		public string OriginalFileName { get; set; } = string.Empty;

		public string? StoredFileName { get; set; }

		public string FileUrl { get; set; } = string.Empty;

		public string? ContentType { get; set; }

		public long? FileSize { get; set; }

		public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

		public ICollection<FreelancerProfileAttachment> FreelancerProfileAttachments { get; set; } = new List<FreelancerProfileAttachment>();

		public ICollection<ClientProfileAttachment> ClientProfileAttachments { get; set; } = new List<ClientProfileAttachment>();

		public ICollection<JobAttachment> JobAttachments { get; set; } = new List<JobAttachment>();

		public ICollection<ApplicationAttachment> ApplicationAttachments { get; set; } = new List<ApplicationAttachment>();

		public ICollection<ContractAttachment> ContractAttachments { get; set; } = new List<ContractAttachment>();
	}
}
