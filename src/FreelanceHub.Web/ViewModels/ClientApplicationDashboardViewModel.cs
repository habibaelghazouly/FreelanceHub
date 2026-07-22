using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Web.ViewModels
{
    public class ClientApplicationDashboardViewModel
    {
        public int? JobId { get; set; }
        public IReadOnlyList<ClientApplicationItemViewModel> Applications { get; set; } = Array.Empty<ClientApplicationItemViewModel>();
    }

    public class ClientApplicationItemViewModel
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
