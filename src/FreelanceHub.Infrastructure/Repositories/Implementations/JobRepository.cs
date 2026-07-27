using FreelanceHub.Infrastructure.Repositories.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FreelanceHub.Infrastructure.Repositories.Implementations
{

    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction? _transaction;

        public JobRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
                await _transaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
                await _transaction.RollbackAsync(cancellationToken);
        }

        public async Task<int> CreateJobAsync(Job job, CancellationToken cancellationToken)
        {
            _dbContext.Jobs.Add(job);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return job.JobId;
        }

        public async Task AddJobCategoriesAsync(IEnumerable<JobCategory> categories,
            CancellationToken cancellationToken)
        {
            await _dbContext.JobCategories.AddRangeAsync(categories, cancellationToken);
        }

        public async Task AddJobSkillsAsync(IEnumerable<JobSkill> skills,
            CancellationToken cancellationToken)
        {
            await _dbContext.JobSkills.AddRangeAsync(skills, cancellationToken);
        }

        public async Task AddJobTagsAsync(IEnumerable<JobTag> tags,
            CancellationToken cancellationToken)
        {
            await _dbContext.JobTags.AddRangeAsync(tags, cancellationToken);
        }

        public async Task AddAttachmentsAsync(IEnumerable<JobAttachment> attachments,
            CancellationToken cancellationToken)
        {
            await _dbContext.JobAttachments.AddRangeAsync(attachments, cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
            => _dbContext.SaveChangesAsync(cancellationToken);

        public async Task<Job?> GetJobByIdAsync(int jobId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Jobs
                .Include(j => j.JobCategories)
                    .ThenInclude(jc => jc.Category)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                .Include(j => j.JobTags)
                    .ThenInclude(jt => jt.Tag)
                .Include(j => j.JobAttachments)
                    .ThenInclude(ja => ja.Attachment)
                .FirstOrDefaultAsync(j => j.JobId == jobId, cancellationToken);
        }

        public async Task<IEnumerable<Job>> GetJobsByClientIdAsync(
            int clientId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Jobs
                .Where(j => j.ClientUserId == clientId)
                .Include(j => j.JobCategories)
                    .ThenInclude(jc => jc.Category)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                .Include(j => j.JobTags)
                    .ThenInclude(jt => jt.Tag)
                .Include(j => j.Applications)
                    .ThenInclude(a => a.FreelancerUser)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Job>> GetAllOpeningJobsAsync(
            CancellationToken cancellationToken)
        {
            return await _dbContext.Jobs
                .Where(j => j.JobStatus == JobStatus.Open)
                .Include(j => j.JobCategories)
                    .ThenInclude(jc => jc.Category)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                .Include(j => j.JobTags)
                    .ThenInclude(jt => jt.Tag)
                .ToListAsync(cancellationToken);
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
            IQueryable<Job> query = _dbContext.Jobs
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

            query = sortOrder switch
            {
                "budget_asc" => query.OrderBy(j => j.Budget),
                "budget_desc" => query.OrderByDescending(j => j.Budget),
                "deadline" => query.OrderBy(j => j.Deadline),
                _ => query.OrderByDescending(j => j.CreatedAt)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var jobs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (jobs, totalCount);
        }

        public async Task<IEnumerable<Job>> GetExpiredJobsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Jobs
                .Where(j => j.Deadline < DateTime.UtcNow && j.JobStatus == JobStatus.Open)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateJobAsync(Job job, CancellationToken cancellationToken = default)
        {
            _dbContext.Jobs.Update(job);
        }

        
    }
}