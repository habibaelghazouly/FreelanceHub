using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Application.DTOs.Results
{
    public class FreelancerApplicationListItemResult
    {
        public int ApplicationId { get; set; }

        public int JobId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public decimal ProposedAmount { get; set; }

        public int TimelineDays { get; set; }

        public ApplicationStatus ApplicationStatus { get; set; }

        public int PortfolioItemCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
