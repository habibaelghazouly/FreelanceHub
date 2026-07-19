using FreelanceHub.Application.DTOs.Results;

namespace FreelanceHub.Application.Services.Abstractions
{
	public interface IProfileService
	{
		Task<UserProfileResult?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
	}
}
