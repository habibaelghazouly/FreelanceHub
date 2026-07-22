using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Domain.Models
{
    public class Contract
    {

        public int ContractId { get; set; }

        // FK
        public int JobId { get; set; }
        public int AcceptedApplicationId { get; set; }

        public decimal AgreedAmount { get; set; }
        public ContractStatus ContractStatus { get; set; } = ContractStatus.Draft;
        public DateTime StartDate { get; set; }
        public DateTime? ExpectedCompletionDate { get; set; }
        public DateTime? ActualCompletionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Job Job { get; set; } = null!;

        public Application AcceptedApplication { get; set; } = null!;
        public ICollection<ContractAttachment> ContractAttachments { get; set; } = new List<ContractAttachment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
