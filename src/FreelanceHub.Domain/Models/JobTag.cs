namespace FreelanceHub.Domain.Models
{
	public class JobTag
	{
		public int JobId { get; set; }

		public Job Job { get; set; } = null!;

		public int TagId { get; set; }

		public Tag Tag { get; set; } = null!;
	}
}
