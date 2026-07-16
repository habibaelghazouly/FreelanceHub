using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;

namespace FreelanceHub.Infrastructure.Repositories.Implementations
{
	public class ClientProfileRepository : IClientProfileRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public ClientProfileRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(ClientProfile clientProfile, CancellationToken cancellationToken = default)
		{
			await _dbContext.ClientProfiles.AddAsync(clientProfile, cancellationToken);
		}
	}
}
