using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Domain.Models
{
    public class Application
    {
        public int ApplicationId { get; set; }

        // FK
        public int JobId { get; set; }
        public int FreelancerUserId { get; set; }


        public decimal ProposedAmount { get; set; }
        public string CoverLetter { get; set; } = null!;

        public int TimelineDays { get; set; } // Timeline in days
        public ApplicationStatus ApplicationStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Job Job { get; set; } = null!;
        
        public ApplicationUser FreelancerUser { get; set; } = null!;

        public Contract? Contract { get; set; } // Navigation property for the associated contract

        public ICollection<ApplicationAttachment> ApplicationAttachments { get; set; } = new List<ApplicationAttachment>();

        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    }
}
