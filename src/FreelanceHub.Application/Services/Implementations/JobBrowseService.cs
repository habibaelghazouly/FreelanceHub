using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;

namespace FreelanceHub.Application.Services.Implementations;

public class JobBrowseService : IJobBrowseService
{
    public IQueryable<Job> ApplyFiltersAndSorting(
        IQueryable<Job> jobs,
        IQueryable<JobSkill> jobSkills,
        int? categoryId,
        decimal? maxBudget,
        int? skillId,
        string sortOrder,
        int pageNumber,
        int pageSize)
    {
        var query = jobs
            .Where(job => !job.IsDeleted && job.JobStatus == JobStatus.Open)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(job => job.CategoryId == categoryId.Value);
        }

        if (maxBudget.HasValue)
        {
            query = query.Where(job => job.Budget <= maxBudget.Value);
        }

        if (skillId.HasValue)
        {
            var skillJobIds = jobSkills
                .Where(js => js.SkillId == skillId.Value)
                .Select(js => js.JobId)
                .ToList();

            query = query.Where(job => skillJobIds.Contains(job.JobId));
        }

        query = sortOrder switch
        {
            "budget" => query.OrderByDescending(job => job.Budget),
            _ => query.OrderByDescending(job => job.CreatedAt)
        };

        var safePageNumber = Math.Max(pageNumber, 1);
        var safePageSize = Math.Max(pageSize, 1);

        return query
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize);
    }
}
