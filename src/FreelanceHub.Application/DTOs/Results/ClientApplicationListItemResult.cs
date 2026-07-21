using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Application.DTOs.Results
{
    public class ClientApplicationListItemResult
    {
        public int ApplicationId { get; set; }

        public int JobId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public int FreelancerUserId { get; set; }

        public string FreelancerDisplayName { get; set; } = string.Empty;

        public decimal ProposedAmount { get; set; }

        public int TimelineDays { get; set; }

        public ApplicationStatus ApplicationStatus { get; set; }

        public DateTime SubmittedAt { get; set; }
    }
}
