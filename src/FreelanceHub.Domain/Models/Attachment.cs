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
	}
}
