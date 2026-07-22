using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Repositories.Implementations
{
	public class ContractRepository : IContractRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public ContractRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public Task<Contract?> GetForParticipantAsync(int contractId, int userId)
		{
			return _dbContext.Contracts
				.Include(contract => contract.Job)
					.ThenInclude(job => job.ClientUser)
				.Include(contract => contract.AcceptedApplication)
					.ThenInclude(application => application.FreelancerUser)
				.Include(contract => contract.Reviews)
				.SingleOrDefaultAsync(contract =>
					contract.ContractId == contractId
					&& (contract.Job.ClientUserId == userId
						|| contract.AcceptedApplication.FreelancerUserId == userId));
		}

		public async Task<IReadOnlyList<Review>> ListReceivedReviewsAsync(int userId)
		{
			return await _dbContext.Reviews
				.AsNoTracking()
				.Where(review => review.RevieweeUserId == userId)
				.Include(review => review.ReviewerUser)
					.ThenInclude(user => user.ProfileImageAttachment)
				.Include(review => review.Contract)
					.ThenInclude(contract => contract.Job)
						.ThenInclude(job => job.JobSkills)
							.ThenInclude(jobSkill => jobSkill.Skill)
				.OrderByDescending(review => review.CreatedAt)
				.ToListAsync();
		}

		public async Task AddReviewAsync(Review review)
		{
			await _dbContext.Reviews.AddAsync(review);
		}
	}
}
