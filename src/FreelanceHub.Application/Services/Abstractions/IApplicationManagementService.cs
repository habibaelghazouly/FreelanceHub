using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Domain.Models;

namespace FreelanceHub.Application.Services.Abstractions
{
    public interface IApplicationManagementService
    {
        Task<ApplicationActionResult> SubmitApplicationAsync(SubmitApplicationRequest request, CancellationToken cancellationToken = default);

        Task<Job?> GetOpenJobByIdAsync(int jobId, CancellationToken cancellationToken = default);

        Task<FreelancerApplicationDashboardResult> GetFreelancerDashboardAsync(int freelancerUserId, CancellationToken cancellationToken = default);

        Task<ClientApplicationDashboardResult> GetClientDashboardAsync(int clientUserId, int jobId, CancellationToken cancellationToken = default);

        Task<ApplicationActionResult> UpdateApplicationStatusAsync(UpdateApplicationStatusRequest request, CancellationToken cancellationToken = default);

        Task<List<FreelanceHub.Domain.Models.Application>> GetApplicationsForJobAsync(int jobId, int clientUserId, CancellationToken cancellationToken = default);
    }
}
