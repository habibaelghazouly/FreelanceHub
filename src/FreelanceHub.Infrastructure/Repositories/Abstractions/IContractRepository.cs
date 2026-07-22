using FreelanceHub.Domain.Models;

namespace FreelanceHub.Infrastructure.Repositories.Abstractions
{
	public interface IContractRepository
	{
		Task<Contract?> GetForParticipantAsync(int contractId, int userId);

		Task<IReadOnlyList<Review>> ListReceivedReviewsAsync(int userId);

		Task AddReviewAsync(Review review);
	}
}
