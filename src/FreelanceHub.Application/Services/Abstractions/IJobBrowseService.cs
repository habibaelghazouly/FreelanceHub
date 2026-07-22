using FreelanceHub.Domain.Models;

namespace FreelanceHub.Application.Services.Abstractions;

public interface IJobBrowseService
{
    IQueryable<Job> ApplyFiltersAndSorting(
        IQueryable<Job> jobs,
        IQueryable<JobSkill> jobSkills,
        int? categoryId,
        decimal? maxBudget,
        int? skillId,
        string sortOrder,
        int pageNumber,
        int pageSize);
}
