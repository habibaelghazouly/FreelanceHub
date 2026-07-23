namespace  FreelanceHub.Application.DTOs.Results
{
    public class ContractListResult
    {
        public int ContractId { get; set; }

        public string JobTitle { get; set; } = string.Empty;
        public string FreelancerName { get; set; } = string.Empty;
        public decimal AgreedAmount { get; set; }

        public string JobDescription { get; set; } = string.Empty;

        public string ContractStatus { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? ExpectedCompletionDate { get; set; }
        public DateTime? ActualCompletionDate { get; set; }
        public string ClientDisplayName { get; set; } = string.Empty;

        public int ClientUserId { get; set; }
        public int FreelancerUserId { get; set; }
        public string FreelancerDisplayName { get; set; } = string.Empty;
    }
}