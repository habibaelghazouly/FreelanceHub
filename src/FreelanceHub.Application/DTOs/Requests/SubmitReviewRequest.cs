namespace FreelanceHub.Application.DTOs.Requests
{
	public class SubmitReviewRequest
	{
		public int Rating { get; set; }

		public string? Comment { get; set; }
	}
}
