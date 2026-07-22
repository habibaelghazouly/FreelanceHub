using FreelanceHub.Domain.Enums;
using System;
using System.Collections.Generic;

namespace FreelanceHub.Web.ViewModels
{
    public class SubmittedApplicationsListViewModel
    {
        public int JobId { get; set; }
        public string? JobTitle { get; set; }
        public List<SubmittedApplicationViewModel> Applications { get; set; } = new();
    }

    public class SubmittedApplicationViewModel
    {
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public int FreelancerUserId { get; set; }
        public string? FreelancerName { get; set; }
        public decimal ProposedAmount { get; set; }
        public int TimelineDays { get; set; }
        public string? CoverLetter { get; set; }
        public DateTime SubmittedAt { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
    }
}