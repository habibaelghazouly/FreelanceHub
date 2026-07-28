using FreelanceHub.Domain.Models;

namespace FreelanceHub.Infrastructure.Repositories.Abstractions;

public interface IAdminRepository
{
    Task<IReadOnlyList<ApplicationUser>> ListUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Job>> ListJobsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Contract>> ListContractsAsync(CancellationToken cancellationToken = default);
    Task<Job?> GetJobForRevocationAsync(int jobId, CancellationToken cancellationToken = default);
}
