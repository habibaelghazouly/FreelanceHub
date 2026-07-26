using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using System;
using System.Collections.Generic;

namespace FreelanceHub.Web.ViewModels
{
    public class ApplicationDetailsViewModel
    {
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public string? JobTitle { get; set; }
        public int FreelancerUserId { get; set; }
        public string? FreelancerName { get; set; }
        public decimal ProposedAmount { get; set; }
        public int TimelineDays { get; set; }
        public string? CoverLetter { get; set; }
        public DateTime SubmittedAt { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
        public bool IsClient { get; set; }

        public List<ApplicationAttachmentViewModel> Attachments { get; set; } = new();
    }

}