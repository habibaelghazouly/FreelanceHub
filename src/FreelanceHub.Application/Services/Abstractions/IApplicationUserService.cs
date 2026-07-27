using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;

namespace FreelanceHub.Application.Services.Abstractions
{
	public interface IApplicationUserService
	{
		Task<ApplicationUserServiceResult> RegisterClientAsync(RegisterClientRequest request, CancellationToken cancellationToken = default);

		Task<ApplicationUserServiceResult> RegisterFreelancerAsync(RegisterFreelancerRequest request, CancellationToken cancellationToken = default);

		Task<AccountDetailsResult?> GetAccountDetailsAsync(int userId);

		Task<UpdateOperationResult> UpdateAccountDetailsAsync(int userId, UpdateAccountDetailsRequest request);

		Task<UpdateOperationResult> ChangePasswordAsync(int userId, ChangePasswordRequest request);

		Task<PasswordResetTokenResult?> CreatePasswordResetTokenAsync(string email);

		Task<UpdateOperationResult> ResetPasswordAsync(ResetPasswordRequest request);

		Task<ApplicationUserServiceResult> LoginAsync(LoginRequest request);

		Task LogoutAsync();
	}
}
