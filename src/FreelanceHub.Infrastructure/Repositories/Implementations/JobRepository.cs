using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Repositories.Implementations;

public class JobRepository(ApplicationDbContext dbContext) : IJobRepository
{
    public async Task<IReadOnlyList<Category>> ListCategoriesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Categories.AsNoTracking().OrderBy(category => category.Name).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Tag>> ListTagsAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Tags.AsNoTracking().OrderBy(tag => tag.Name).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Skill>> ListSkillsAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Skills.AsNoTracking().OrderBy(skill => skill.Name).ToListAsync(cancellationToken);

    public async Task<bool> AreValidAttributesAsync(IReadOnlyCollection<int> categoryIds, IReadOnlyCollection<int> skillIds, IReadOnlyCollection<int> tagIds, CancellationToken cancellationToken = default)
    {
        var categoryCount = await dbContext.Categories.CountAsync(category => categoryIds.Contains(category.CategoryId), cancellationToken);
        var skillCount = await dbContext.Skills.CountAsync(skill => skillIds.Contains(skill.SkillId), cancellationToken);
        var tagCount = await dbContext.Tags.CountAsync(tag => tagIds.Contains(tag.TagId), cancellationToken);
        return categoryCount == categoryIds.Count && skillCount == skillIds.Count && tagCount == tagIds.Count;
    }

    public Task AddAsync(Job job, CancellationToken cancellationToken = default) => dbContext.Jobs.AddAsync(job, cancellationToken).AsTask();

    public void AddAttributes(Job job, IEnumerable<int> categoryIds, IEnumerable<int> skillIds, IEnumerable<int> tagIds)
    {
        dbContext.JobCategories.AddRange(categoryIds.Select(id => new JobCategory { JobId = job.JobId, CategoryId = id }));
        dbContext.JobSkills.AddRange(skillIds.Select(id => new JobSkill { JobId = job.JobId, SkillId = id }));
        dbContext.JobTags.AddRange(tagIds.Select(id => new JobTag { JobId = job.JobId, TagId = id }));
    }

    public void AddAttachment(JobAttachment attachment) => dbContext.JobAttachments.Add(attachment);

    public async Task<IReadOnlyList<Job>> ListByClientIdAsync(int clientId, CancellationToken cancellationToken = default) =>
        await JobDetails().Where(job => job.ClientUserId == clientId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Job>> ListOpenAsync(CancellationToken cancellationToken = default) =>
        await JobDetails().Where(job => !job.IsDeleted && job.JobStatus == JobStatus.Open).ToListAsync(cancellationToken);

    public Task<Job?> GetByIdAsync(int jobId, CancellationToken cancellationToken = default) =>
        JobDetails().FirstOrDefaultAsync(job => job.JobId == jobId, cancellationToken);

    public async Task<(IReadOnlyList<Job> Jobs, int TotalCount)> BrowseOpenAsync(int? categoryId, decimal? maxBudget, int? skillId, string sortOrder, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Jobs.AsNoTracking().Where(job => !job.IsDeleted && job.JobStatus == JobStatus.Open);
        if (categoryId.HasValue) query = query.Where(job => job.CategoryId == categoryId.Value || job.JobCategories.Any(item => item.CategoryId == categoryId.Value));
        if (maxBudget.HasValue) query = query.Where(job => job.Budget <= maxBudget.Value);
        if (skillId.HasValue) query = query.Where(job => job.JobSkills.Any(item => item.SkillId == skillId.Value));

        var totalCount = await query.CountAsync(cancellationToken);
        query = sortOrder?.Trim().ToLowerInvariant() switch
        {
            "budget" or "budget_desc" => query.OrderByDescending(job => job.Budget),
            "budget_asc" => query.OrderBy(job => job.Budget),
            _ => query.OrderByDescending(job => job.CreatedAt)
        };
        var jobs = await query.Skip((Math.Max(pageNumber, 1) - 1) * Math.Clamp(pageSize, 1, 100)).Take(Math.Clamp(pageSize, 1, 100)).ToListAsync(cancellationToken);
        return (jobs, totalCount);
    }

    private IQueryable<Job> JobDetails() => dbContext.Jobs
        .AsNoTracking()
        .Include(job => job.JobCategories).ThenInclude(item => item.Category)
        .Include(job => job.JobSkills).ThenInclude(item => item.Skill)
        .Include(job => job.JobTags).ThenInclude(item => item.Tag)
        .Include(job => job.JobAttachments).ThenInclude(item => item.Attachment)
        .Include(job => job.Applications).ThenInclude(item => item.FreelancerUser);

    public async Task<IEnumerable<Job>> GetExpiredJobsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs
            .Where(j => j.Deadline < DateTime.UtcNow && j.JobStatus == JobStatus.Open)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateJobAsync(Job job, CancellationToken cancellationToken = default)
    {
        dbContext.Jobs.Update(job);
    }

    public async Task<(IReadOnlyList<Job> Jobs, int TotalCount)> BrowseJobsAsync(
        int? categoryId,
        int? skillId,
        decimal? maxBudget,
        string? sortOrder,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Job> query = dbContext.Jobs
            .Include(j => j.JobCategories)
                .ThenInclude(jc => jc.Category)
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .Include(j => j.JobTags)
                .ThenInclude(jt => jt.Tag)
            .Where(j => j.JobStatus == JobStatus.Open && !j.IsDeleted);

        if (categoryId.HasValue)
        {
            query = query.Where(j =>
                j.JobCategories.Any(c => c.CategoryId == categoryId.Value));
        }

        if (skillId.HasValue)
        {
            query = query.Where(j =>
                j.JobSkills.Any(s => s.SkillId == skillId.Value));
        }

        if (maxBudget.HasValue)
        {
            query = query.Where(j => j.Budget <= maxBudget.Value);
        }

        // Normalize so "budget", "Budget", "budget_desc", etc. all resolve consistently,
        // and align this with BrowseOpenAsync's accepted values.
        query = sortOrder?.Trim().ToLowerInvariant() switch
        {
            "budget" or "budget_desc" => query.OrderByDescending(j => j.Budget),
            "budget_asc" => query.OrderBy(j => j.Budget),
            "deadline" => query.OrderBy(j => j.Deadline),
            _ => query.OrderByDescending(j => j.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var jobs = await query
            .Skip((Math.Max(pageNumber, 1) - 1) * Math.Clamp(pageSize, 1, 100))
            .Take(Math.Clamp(pageSize, 1, 100))
            .ToListAsync(cancellationToken);

        return (jobs, totalCount);
    }
}