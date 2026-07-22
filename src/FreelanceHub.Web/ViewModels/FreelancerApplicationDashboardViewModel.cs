using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Web.ViewModels
{
    public class FreelancerApplicationDashboardViewModel
    {
        public IReadOnlyList<FreelancerApplicationItemViewModel> Applications { get; set; } = Array.Empty<FreelancerApplicationItemViewModel>();

    }

    public class FreelancerApplicationItemViewModel
    {
        public int ApplicationId { get; set; }

        public int JobId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public decimal ProposedAmount { get; set; }

        public int TimelineDays { get; set; }

        public ApplicationStatus ApplicationStatus { get; set; }

        public int PortfolioItemCount { get; set; }

        public DateTime SubmittedAt { get; set; }
    }

}
