using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;

namespace FreelanceHub.Application.Services.Abstractions
{
	public interface IContractService
	{
		Task<ContractDetailsResult?> GetDetailsAsync(int contractId, int currentUserId);

		Task<UpdateOperationResult> CompleteAsync(int contractId, int currentUserId);

		Task<UpdateOperationResult> TerminateAsync(int contractId, int currentUserId);

		Task<UpdateOperationResult> SubmitReviewAsync(int contractId, int currentUserId, SubmitReviewRequest request);

		Task<IReadOnlyList<ReceivedReviewResult>> GetReceivedReviewsAsync(int userId);

		Task<IReadOnlyList<ContractListResult>> GetContractsForUserAsync(int userId);

		Task<UpdateOperationResult> CompleteAsync(int contractId, int freelancerUserId);

		Task<UpdateOperationResult> TerminateAsync(int contractId, int userId);
	}
}
