using FreelanceHub.Domain.Models;

namespace FreelanceHub.Infrastructure.Repositories.Abstractions
{
    public interface IApplicationRepository
    {
        Task<Job?> GetOpenJobByIdAsync(int jobId, CancellationToken cancellationToken = default);

        Task<bool> HasFreelancerAppliedAsync(int jobId, int freelancerUserId, CancellationToken cancellationToken = default);

        Task AddAsync(Application application, CancellationToken cancellationToken = default);

        Task<List<Application>> ListByFreelancerUserIdAsync(int freelancerUserId, CancellationToken cancellationToken = default);

        Task<List<Application>> ListByClientUserIdAsync(int clientUserId, CancellationToken cancellationToken = default);

        Task<Application?> GetByIdForClientAsync(int applicationId, int clientUserId, CancellationToken cancellationToken = default);

        Task<List<Application>> GetApplicationsByJobIdAsync(int jobId, CancellationToken cancellationToken = default);
    }
}
