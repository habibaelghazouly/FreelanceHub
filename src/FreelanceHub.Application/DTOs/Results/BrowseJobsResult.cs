using FreelanceHub.Domain.Models;

namespace FreelanceHub.Application.DTOs.Results
{
   public class BrowseJobsResult
{
    public IReadOnlyList<Job> Jobs { get; set; } = [];

    public int TotalCount { get; set; }

    public IReadOnlyList<Category> Categories { get; set; } = [];

    public IReadOnlyList<Skill> Skills { get; set; } = [];
}
}