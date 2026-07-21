namespace FreelanceHub.Application.DTOs.Results
{
    public class FreelancerApplicationDashboardResult
    {
        public IReadOnlyList<FreelancerApplicationListItemResult> Applications { get; set; } = Array.Empty<FreelancerApplicationListItemResult>();
    }
}
