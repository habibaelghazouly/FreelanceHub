using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;

namespace FreelanceHub.Infrastructure.Repositories.Implementations
{
	public class FreelancerProfileRepository : IFreelancerProfileRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public FreelancerProfileRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(FreelancerProfile freelancerProfile, CancellationToken cancellationToken = default)
		{
			await _dbContext.FreelancerProfiles.AddAsync(freelancerProfile, cancellationToken);
		}
	}
}
