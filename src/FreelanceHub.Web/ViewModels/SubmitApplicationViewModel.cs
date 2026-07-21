using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FreelanceHub.Web.ViewModels
{
    public class SubmitApplicationViewModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int JobId { get; set; }

        [Display(Name = "Job title")]
        public string? JobTitle { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "9999999999999999.99")]
        [Display(Name = "Bid amount")]
        public decimal? ProposedAmount { get; set; }

        [Required]
        [StringLength(4000, MinimumLength = 20)]
        [Display(Name = "Cover letter")]
        public string CoverLetter { get; set; } = string.Empty;

        [Required]
        [Range(1, 3650)]
        [Display(Name = "Timeline (days)")]
        public int? TimelineDays { get; set; }

        [Display(Name = "Portfolio files (PDF/images)")]
        public List<IFormFile> PortfolioFiles { get; set; } = new();
    }
}
