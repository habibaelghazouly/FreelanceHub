namespace FreelanceHub.Application.DTOs.Requests
{
    public class SubmitApplicationRequest
    {
        public int JobId { get; set; }

        public int FreelancerUserId { get; set; }

        public decimal ProposedAmount { get; set; }

        public string CoverLetter { get; set; } = string.Empty;

        public int TimelineDays { get; set; }

        public IReadOnlyList<UploadedFileRequest> PortfolioFiles { get; set; } = Array.Empty<UploadedFileRequest>();
    }
}
