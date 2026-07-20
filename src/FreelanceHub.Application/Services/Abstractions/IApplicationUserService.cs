using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;

namespace FreelanceHub.Application.Services.Abstractions
{
	public interface IApplicationUserService
	{
		Task<ApplicationUserServiceResult> RegisterClientAsync(RegisterClientRequest request, CancellationToken cancellationToken = default);

		Task<ApplicationUserServiceResult> RegisterFreelancerAsync(RegisterFreelancerRequest request, CancellationToken cancellationToken = default);

		Task<ApplicationUserServiceResult> LoginAsync(LoginRequest request);

		Task LogoutAsync();
	}
}
