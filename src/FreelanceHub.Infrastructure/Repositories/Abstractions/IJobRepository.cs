using FreelanceHub.Domain.Models;

namespace FreelanceHub.Infrastructure.Repositories.Abstractions
{
    public interface IJobRepository
    {
        Task<int> CreateJobAsync(Job job, CancellationToken cancellationToken);

        Task AddJobCategoriesAsync(IEnumerable<JobCategory> categories,
            CancellationToken cancellationToken);

        Task AddJobSkillsAsync(IEnumerable<JobSkill> skills,
            CancellationToken cancellationToken);

        Task AddJobTagsAsync(IEnumerable<JobTag> tags,
            CancellationToken cancellationToken);

        Task AddAttachmentsAsync(
            IEnumerable<JobAttachment> attachments,
            CancellationToken cancellationToken);

        Task<Job?> GetJobByIdAsync(int jobId,
            CancellationToken cancellationToken);

        Task<IEnumerable<Job>> GetJobsByClientIdAsync(
            int clientId,
            CancellationToken cancellationToken);

        Task<IEnumerable<Job>> GetAllOpeningJobsAsync(
            CancellationToken cancellationToken);

        Task BeginTransactionAsync(CancellationToken cancellationToken);

        Task CommitTransactionAsync(CancellationToken cancellationToken);

        Task RollbackTransactionAsync(CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);



        Task<(IReadOnlyList<Job> Jobs, int TotalCount)> BrowseJobsAsync(
            int? categoryId,
            int? skillId,
            decimal? maxBudget,
            string? sortOrder,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Job>> GetExpiredJobsAsync(CancellationToken cancellationToken = default);

        Task UpdateJobAsync(Job job, CancellationToken cancellationToken = default);


    }
}