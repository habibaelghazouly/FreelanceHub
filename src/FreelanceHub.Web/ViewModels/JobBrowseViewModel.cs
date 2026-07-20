using FreelanceHub.Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreelanceHub.Web.ViewModels;

public class JobBrowseViewModel
{
    public string SortOrder { get; set; } = "date";
    public int? CategoryId { get; set; }
    public int? SkillId { get; set; }
    public decimal? MaxBudget { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 6;
    public int TotalCount { get; set; }
    public IReadOnlyList<Job> Jobs { get; set; } = Array.Empty<Job>();
    public IReadOnlyList<SelectListItem> Categories { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Skills { get; set; } = Array.Empty<SelectListItem>();
}
