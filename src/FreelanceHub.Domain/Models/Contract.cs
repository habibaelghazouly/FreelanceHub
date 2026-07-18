using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Domain.Models
{
    public class Contract
    {

        public int ContractId { get; set; }

        // FK
        public int JobId { get; set; }
        public int ApplicationId { get; set; }

        public decimal AgreedAmount { get; set; }
        public ContractStatus ContractStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpectedCompletionDate { get; set; }
        public DateTime? ActualCompletionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Job Job { get; set; } = null!;

        public Application Application { get; set; } = null!;
        public ICollection<ContractAttachment> ContractAttachments { get; set; } = new List<ContractAttachment>();
        //public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
