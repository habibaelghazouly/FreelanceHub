using Microsoft.AspNetCore.Identity;

namespace FreelanceHub.Domain.Models
{
	public class ApplicationUser : IdentityUser<int>
	{
		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public int UserStatus { get; set; }

		public int? ProfileImageAttachmentId { get; set; }

		public Attachment? ProfileImageAttachment { get; set; }

		public ClientProfile? ClientProfile { get; set; }

		public FreelancerProfile? FreelancerProfile { get; set; }

		public ICollection<Attachment> UploadedAttachments { get; set; } = new List<Attachment>();

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
	}
}
