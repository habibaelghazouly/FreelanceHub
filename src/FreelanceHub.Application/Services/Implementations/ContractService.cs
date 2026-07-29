using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Application.Services.Implementations
{
	public class ContractService : IContractService
	{
		private readonly IContractRepository _contractRepository;
		private readonly IApplicationUserRepository _applicationUserRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly INotificationService _notificationService;
		private readonly INotificationPublisher _notificationPublisher;

		public ContractService(
			IContractRepository contractRepository,
			IApplicationUserRepository applicationUserRepository,
			IUnitOfWork unitOfWork,
			INotificationService notificationService,
			INotificationPublisher notificationPublisher)
		{
			_contractRepository = contractRepository;
			_applicationUserRepository = applicationUserRepository;
			_unitOfWork = unitOfWork;
			_notificationService = notificationService;
			_notificationPublisher = notificationPublisher;
		}

		public async Task<ContractDetailsResult?> GetDetailsAsync(int contractId, int currentUserId)
		{
			var contract = await _contractRepository.GetForParticipantAsync(contractId, currentUserId);
			if (contract is null)
			{
				return null;
			}

			var clientUser = contract.Job.ClientUser;
			var freelancerUser = contract.AcceptedApplication.FreelancerUser;
			var hasSubmittedReview = contract.Reviews.Any(review => review.ReviewerUserId == currentUserId);
			var canReviewStatus = contract.ContractStatus is ContractStatus.Completed or ContractStatus.Terminated;
			var reviewee = currentUserId == clientUser.Id ? freelancerUser : clientUser;

			return new ContractDetailsResult
			{
				ContractId = contract.ContractId,
				AcceptedApplicationId = contract.AcceptedApplicationId,
				JobId = contract.JobId,
				JobTitle = contract.Job.Title,
				AgreedAmount = contract.AgreedAmount,
				ContractStatus = contract.ContractStatus,
				StartDate = contract.StartDate,
				ExpectedCompletionDate = contract.ExpectedCompletionDate,
				ActualCompletionDate = contract.ActualCompletionDate,
				ClientUserId = clientUser.Id,
				ClientDisplayName = GetDisplayName(clientUser),
				FreelancerUserId = freelancerUser.Id,
				FreelancerDisplayName = GetDisplayName(freelancerUser),
				RevieweeUserId = reviewee.Id,
				RevieweeDisplayName = GetDisplayName(reviewee),
				CanSubmitReview = canReviewStatus && !hasSubmittedReview,
				HasSubmittedReview = hasSubmittedReview
			};
		}

		public async Task<UpdateOperationResult> SubmitReviewAsync(
			int contractId,
			int currentUserId,
			SubmitReviewRequest request)
		{
			if (request.Rating is < 1 or > 5)
			{
				return UpdateOperationResult.Failed(
					new UpdateOperationError(nameof(request.Rating), "Rating must be between 1 and 5."));
			}

			var comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();
			if (comment?.Length > 1000)
			{
				return UpdateOperationResult.Failed(
					new UpdateOperationError(nameof(request.Comment), "Review comment cannot exceed 1000 characters."));
			}

			var contract = await _contractRepository.GetForParticipantAsync(contractId, currentUserId);
			if (contract is null)
			{
				return UpdateOperationResult.Missing();
			}

			if (contract.ContractStatus is not (ContractStatus.Completed or ContractStatus.Terminated))
			{
				return UpdateOperationResult.Failed(
					new UpdateOperationError(null, "Reviews can only be submitted for completed or terminated contracts."));
			}

			if (contract.Reviews.Any(review => review.ReviewerUserId == currentUserId))
			{
				return UpdateOperationResult.Failed(
					new UpdateOperationError(null, "You have already reviewed this contract."));
			}

			var clientUserId = contract.Job.ClientUserId;
			var freelancerUserId = contract.AcceptedApplication.FreelancerUserId;
			var revieweeUserId = currentUserId == clientUserId ? freelancerUserId : clientUserId;
			var reviewee = await _applicationUserRepository.GetWithProfileForUpdateAsync(revieweeUserId);
			if (reviewee is null)
			{
				return UpdateOperationResult.Missing();
			}

			if (currentUserId == clientUserId && reviewee.FreelancerProfile is not null)
			{
				UpdateRating(reviewee.FreelancerProfile, request.Rating);
			}
			else if (currentUserId == freelancerUserId && reviewee.ClientProfile is not null)
			{
				UpdateRating(reviewee.ClientProfile, request.Rating);
			}
			else
			{
				return UpdateOperationResult.Failed(
					new UpdateOperationError(null, "The reviewed user's profile could not be found."));
			}

			await _contractRepository.AddReviewAsync(new Review
			{
				ContractId = contractId,
				ReviewerUserId = currentUserId,
				RevieweeUserId = revieweeUserId,
				Rating = request.Rating,
				Comment = comment
			});
			await _notificationService.CreateAsync(new CreateNotificationRequest
			{
				RecipientUserId = revieweeUserId,
				ActorUserId = currentUserId,
				NotificationType = NotificationType.ReviewReceived,
				Title = "New review received",
				Message = $"You received a {request.Rating}-star review for {contract.Job.Title}.",
				TargetUrl = $"/profile/{revieweeUserId}#reviews",
				RelatedEntityId = contractId
			});

			try
			{
				await _unitOfWork.SaveChangesAsync();
				await _notificationPublisher.NotifyChangedAsync(revieweeUserId);
				return UpdateOperationResult.Success();
			}
			catch (DbUpdateException)
			{
				return UpdateOperationResult.Failed(
					new UpdateOperationError(null, "Unable to save this review. Please try again."));
			}
		}

		public async Task<IReadOnlyList<ReceivedReviewResult>> GetReceivedReviewsAsync(int userId)
		{
			var reviews = await _contractRepository.ListReceivedReviewsAsync(userId);
			return reviews.Select(review => new ReceivedReviewResult
			{
				ContractId = review.ContractId,
				JobTitle = review.Contract.Job.Title,
				ReviewerUserId = review.ReviewerUserId,
				ReviewerDisplayName = GetDisplayName(review.ReviewerUser),
				ReviewerProfileImageUrl = review.ReviewerUser.ProfileImageAttachment?.FileUrl,
				Rating = review.Rating,
				ProjectPrice = review.Contract.AgreedAmount,
				ProjectSkills = review.Contract.Job.JobSkills
					.Select(jobSkill => jobSkill.Skill.Name)
					.OrderBy(skillName => skillName)
					.ToArray(),
				Comment = review.Comment,
				CreatedAt = review.CreatedAt
			}).ToArray();
		}

		private static void UpdateRating(FreelancerProfile profile, int rating)
		{
			var newCount = profile.RatingCount + 1;
			profile.RatingAverage = CalculateAverage(profile.RatingAverage, profile.RatingCount, rating, newCount);
			profile.RatingCount = newCount;
		}

		private static void UpdateRating(ClientProfile profile, int rating)
		{
			var newCount = profile.RatingCount + 1;
			profile.RatingAverage = CalculateAverage(profile.RatingAverage, profile.RatingCount, rating, newCount);
			profile.RatingCount = newCount;
		}

		private static decimal CalculateAverage(decimal currentAverage, int currentCount, int rating, int newCount)
		{
			return Math.Round(((currentAverage * currentCount) + rating) / newCount, 2);
		}

		private static string GetDisplayName(ApplicationUser user)
		{
			var fullName = $"{user.FirstName} {user.LastName}".Trim();
			return string.IsNullOrWhiteSpace(fullName) ? user.UserName ?? "User" : fullName;
		}

		public async Task<IReadOnlyList<ContractListResult>> GetContractsForUserAsync(int userId)
		{
			var contracts = await _contractRepository.ListContractsForUserAsync(userId);
			return contracts.Select(contract => new ContractListResult
			{

				ContractId = contract.ContractId,
				AcceptedApplicationId = contract.AcceptedApplicationId,
				JobTitle = contract.Job.Title,
				AgreedAmount = contract.AgreedAmount,
				ContractStatus = GetContractStatusDisplayName(contract.ContractStatus),
				StartDate = contract.StartDate,
				ExpectedCompletionDate = contract.ExpectedCompletionDate,
				ActualCompletionDate = contract.ActualCompletionDate,
				ClientDisplayName = GetDisplayName(contract.Job.ClientUser),
				FreelancerDisplayName = GetDisplayName(contract.AcceptedApplication.FreelancerUser),
				ClientUserId = contract.Job.ClientUserId,
				FreelancerUserId = contract.AcceptedApplication.FreelancerUserId
			}).ToArray();
		}

		public async Task<UpdateOperationResult> CompleteAsync(int contractId, int freelancerUserId)
		{
			var contract = await _contractRepository.GetForParticipantAsync(contractId, freelancerUserId);
			if (contract is null)
			{
				return UpdateOperationResult.Missing();
			}

			if (contract.AcceptedApplication.FreelancerUserId != freelancerUserId)
			{
				return UpdateOperationResult.Failed(new UpdateOperationError(null, "Only the assigned freelancer can complete this contract."));
			}

			if (contract.ContractStatus is not (ContractStatus.Draft or ContractStatus.Accepted))
			{
				return UpdateOperationResult.Failed(new UpdateOperationError(null, "Only active contracts can be completed."));
			}

			contract.ContractStatus = ContractStatus.Completed;
			contract.ActualCompletionDate = DateTime.UtcNow;
			contract.UpdatedAt = DateTime.UtcNow;
			contract.Job.JobStatus = JobStatus.Completed;
			contract.Job.UpdatedAt = DateTime.UtcNow;

			try
			{
				await _notificationService.CreateAsync(new CreateNotificationRequest
				{
					RecipientUserId = contract.Job.ClientUserId,
					ActorUserId = freelancerUserId,
					NotificationType = NotificationType.ContractStatusChanged,
					Title = "Contract completed",
					Message = $"The contract for {contract.Job.Title} was marked completed.",
					TargetUrl = $"/Contract/Details/{contractId}",
					RelatedEntityId = contractId
				});
				await _unitOfWork.SaveChangesAsync();
				await _notificationPublisher.NotifyChangedAsync(contract.Job.ClientUserId);
				return UpdateOperationResult.Success();
			}
			catch (DbUpdateException)
			{
				return UpdateOperationResult.Failed(
					new UpdateOperationError(null, "Unable to complete this contract. Please try again."));
			}
		}

		public async Task<UpdateOperationResult> TerminateAsync(int contractId, int userId)
		{
			var contract = await _contractRepository.GetForParticipantAsync(contractId, userId);
			if (contract is null)
			{
				return UpdateOperationResult.Missing();
			}

			if (contract.ContractStatus is not (ContractStatus.Draft or ContractStatus.Accepted))
			{
				return UpdateOperationResult.Failed(new UpdateOperationError(null, "Only active contracts can be terminated."));
			}

			contract.ContractStatus = ContractStatus.Terminated;
			contract.ActualCompletionDate = DateTime.UtcNow;
			contract.UpdatedAt = DateTime.UtcNow;
			contract.Job.JobStatus = JobStatus.Cancelled;
			contract.Job.UpdatedAt = DateTime.UtcNow;

			var clientUserId = contract.Job.ClientUserId;
			var freelancerUserId = contract.AcceptedApplication.FreelancerUserId;
			var recipientUserId = userId == clientUserId ? freelancerUserId : clientUserId;

			try
			{
				await _notificationService.CreateAsync(new CreateNotificationRequest
				{
					RecipientUserId = recipientUserId,
					ActorUserId = userId,
					NotificationType = NotificationType.ContractStatusChanged,
					Title = "Contract terminated",
					Message = $"The contract for {contract.Job.Title} was terminated.",
					TargetUrl = $"/Contract/Details/{contractId}",
					RelatedEntityId = contractId
				});
				await _unitOfWork.SaveChangesAsync();
				await _notificationPublisher.NotifyChangedAsync(recipientUserId);
				return UpdateOperationResult.Success();
			}
			catch (DbUpdateException)
			{
				return UpdateOperationResult.Failed(
					new UpdateOperationError(null, "Unable to terminate this contract. Please try again."));
			}
		}

		public string GetContractStatusDisplayName(ContractStatus status)
		{
			return status switch
			{
				ContractStatus.Accepted => "Accepted",
				ContractStatus.Rejected => "Rejected",
				ContractStatus.Draft => "Draft",
				ContractStatus.Completed => "Completed",
				ContractStatus.Terminated => "Terminated",
				_ => "Unknown"
			};
		}
	}
}
