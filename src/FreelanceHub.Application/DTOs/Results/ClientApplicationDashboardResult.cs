namespace FreelanceHub.Application.DTOs.Results
{
    public class ClientApplicationDashboardResult
    {
        public IReadOnlyList<ClientApplicationListItemResult> Applications { get; set; } = Array.Empty<ClientApplicationListItemResult>();
    }
}
