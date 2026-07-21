using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;

namespace FreelanceHub.Application.Services.Abstractions
{
    public interface IApplicationManagementService
    {
        Task<ApplicationActionResult> SubmitApplicationAsync(SubmitApplicationRequest request, CancellationToken cancellationToken = default);

        Task<FreelancerApplicationDashboardResult> GetFreelancerDashboardAsync(int freelancerUserId, CancellationToken cancellationToken = default);

        Task<ClientApplicationDashboardResult> GetClientDashboardAsync(int clientUserId, CancellationToken cancellationToken = default);

        Task<ApplicationActionResult> UpdateApplicationStatusAsync(UpdateApplicationStatusRequest request, CancellationToken cancellationToken = default);
    }
}
