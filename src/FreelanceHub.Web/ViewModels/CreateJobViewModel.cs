using System.ComponentModel.DataAnnotations;

namespace FreelanceHub.Web.ViewModels
{
    public class CreateJobViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        [Display(Name = "Job title")]
        public string Title { get; set; } = string.Empty;
        [Display(Name = "Job description")]
        [Required]
        [StringLength(4000, MinimumLength = 20)]
        public string Description { get; set; } = string.Empty;
        [Display(Name = "Budget")]
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Budget must be a positive value")]
        public decimal Budget { get; set; }
        [Display(Name = "Deadline")]
        [Required]
        public DateTime Deadline { get; set; } = DateTime.Now.AddDays(7); // Default to one week from now
        [Display(Name = "Categories")]
        public string? CategoryIds { get; set; } = string.Empty;
        [Display(Name = "Skills")]
        public string? SkillIds { get; set; } = string.Empty;
        [Display(Name = "Tags")]
        public string? TagIds { get; set; } = string.Empty;

        [Display(Name = "Job Files (PDF/Images)")]
        public List<IFormFile> JobFiles { get; set; } = new();
    }
}