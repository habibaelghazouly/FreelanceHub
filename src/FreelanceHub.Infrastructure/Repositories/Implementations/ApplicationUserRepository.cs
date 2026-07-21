using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Repositories.Implementations
{
	public class ApplicationUserRepository : IApplicationUserRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public ApplicationUserRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public Task<ApplicationUser?> GetWithProfileAsync(int userId, CancellationToken cancellationToken = default)
		{
			return _dbContext.Users
				.AsNoTracking()
				.Include(user => user.ProfileImageAttachment)
				.Include(user => user.ClientProfile)
					.ThenInclude(profile => profile!.CompanyLogoAttachment)
				.Include(user => user.FreelancerProfile)
				.SingleOrDefaultAsync(user => user.Id == userId, cancellationToken);
		}

		public Task<ApplicationUser?> GetWithProfileForUpdateAsync(int userId, CancellationToken cancellationToken = default)
		{
			return _dbContext.Users
				.Include(user => user.ClientProfile)
				.Include(user => user.FreelancerProfile)
				.SingleOrDefaultAsync(user => user.Id == userId, cancellationToken);
		}
	}
}
