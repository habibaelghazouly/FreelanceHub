
using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Domain.Models
{
        public class Job
        {
            public int JobId { get; set; }

            // FK
            public int ClientUserId { get; set; }
        public int? CategoryId { get; set; }
           
            public string Title { get; set; } = null!;
            public string Description { get; set; } = null!;
            public decimal Budget { get; set; }
            public DateTime Deadline { get; set; }
            public JobStatus JobStatus { get; set; } = JobStatus.Open;
            public bool IsDeleted { get; set; }
            public DateTime? DeletedAt { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }


        // Navigation properties

        public ApplicationUser ClientUser { get; set; } = null!;
        public Category? Category { get; set; }

        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public Contract? Contract { get; set; }
        public ICollection<JobCategory> JobCategories { get; set; } = new List<JobCategory>();
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        public ICollection<JobTag> JobTags { get; set; } = new List<JobTag>();
        public ICollection<JobAttachment> JobAttachments { get; set; } = new List<JobAttachment>();

    }
}
