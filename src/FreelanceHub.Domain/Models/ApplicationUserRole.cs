using Microsoft.AspNetCore.Identity;

namespace FreelanceHub.Domain.Models
{
	public class ApplicationUserRole : IdentityUserRole<int>
	{
		public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
	}
}
