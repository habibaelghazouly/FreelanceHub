using FreelanceHub.Domain.Models;

namespace FreelanceHub.Infrastructure.Repositories.Abstractions
{
	public interface IFreelancerProfileRepository
	{
		Task AddAsync(FreelancerProfile freelancerProfile, CancellationToken cancellationToken = default);
	}
}
