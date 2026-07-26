namespace FreelanceHub.Web.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int FreelancerCount { get; set; }
        public int ClientCount { get; set; }
        public int JobCount { get; set; }
        public int ContractCount { get; set; }
        public IReadOnlyList<AdminJobViewModel> Jobs { get; set; } = Array.Empty<AdminJobViewModel>();
        public IReadOnlyList<AdminContractViewModel> Contracts { get; set; } = Array.Empty<AdminContractViewModel>();
        public IReadOnlyList<AdminUserViewModel> Freelancers { get; set; } = Array.Empty<AdminUserViewModel>();
        public IReadOnlyList<AdminUserViewModel> Clients { get; set; } = Array.Empty<AdminUserViewModel>();
    }

    public class AdminContractViewModel
    {
        public int ContractId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string FreelancerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class AdminJobViewModel
    {
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ApplicationCount { get; set; }
        public bool CanRevoke { get; set; }
    }

    public class AdminUserViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
