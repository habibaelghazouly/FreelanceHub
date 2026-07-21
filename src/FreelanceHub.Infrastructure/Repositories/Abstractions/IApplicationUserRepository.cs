using FreelanceHub.Domain.Models;

namespace FreelanceHub.Infrastructure.Repositories.Abstractions
{
	public interface IApplicationUserRepository
	{
		Task<ApplicationUser?> GetWithProfileAsync(int userId, CancellationToken cancellationToken = default);

		Task<ApplicationUser?> GetWithProfileForUpdateAsync(int userId, CancellationToken cancellationToken = default);
	}
}
