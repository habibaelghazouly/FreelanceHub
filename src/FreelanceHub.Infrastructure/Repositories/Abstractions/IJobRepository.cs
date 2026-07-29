using FreelanceHub.Domain.Models;

namespace FreelanceHub.Infrastructure.Repositories.Abstractions;

public interface IJobRepository
{
    Task<IReadOnlyList<Category>> ListCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tag>> ListTagsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Skill>> ListSkillsAsync(CancellationToken cancellationToken = default);
    Task<bool> AreValidAttributesAsync(IReadOnlyCollection<int> categoryIds, IReadOnlyCollection<int> skillIds, IReadOnlyCollection<int> tagIds, CancellationToken cancellationToken = default);
    Task AddAsync(Job job, CancellationToken cancellationToken = default);
    void AddAttributes(Job job, IEnumerable<int> categoryIds, IEnumerable<int> skillIds, IEnumerable<int> tagIds);
    void AddAttachment(JobAttachment attachment);
    Task<IReadOnlyList<Job>> ListByClientIdAsync(int clientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Job>> ListOpenAsync(CancellationToken cancellationToken = default);
    Task<Job?> GetByIdAsync(int jobId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Job> Jobs, int TotalCount)> BrowseOpenAsync(int? categoryId, decimal? maxBudget, int? skillId, string sortOrder, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<IEnumerable<Job>> GetExpiredJobsAsync(CancellationToken cancellationToken = default);

    Task UpdateJobAsync(Job job, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Job> Jobs, int TotalCount)> BrowseJobsAsync(
            int? categoryId,
            int? skillId,
            decimal? maxBudget,
            string? sortOrder,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
}
