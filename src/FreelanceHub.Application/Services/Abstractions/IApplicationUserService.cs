using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;

namespace FreelanceHub.Application.Services.Abstractions
{
	public interface IApplicationUserService
	{
		Task<ApplicationUserServiceResult> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);

		Task<ApplicationUserServiceResult> LoginAsync(LoginRequest request);

		Task LogoutAsync();
	}
}
