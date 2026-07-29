using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Exceptions;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using ApplicationEntity = FreelanceHub.Domain.Models.Application;

namespace FreelanceHub.Application.Services.Implementations
{
    public class ApplicationManagementService : IApplicationManagementService
    {
        private const string PortfolioUploadsFolder = "application-portfolios";
        private const int MaxPortfolioFiles = 10;
        private static readonly HashSet<ApplicationStatus> AllowedClientStatuses = new()
        {
            ApplicationStatus.UnderReview,
            ApplicationStatus.Accepted,
            ApplicationStatus.Rejected
        };

        private readonly IApplicationRepository _applicationRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;
        private readonly INotificationService _notificationService;
        private readonly INotificationPublisher _notificationPublisher;

        public ApplicationManagementService(
            IApplicationRepository applicationRepository,
            IAttachmentRepository attachmentRepository,
            IFileUploadService fileUploadService,
            IUnitOfWork unitOfWork,
            ApplicationDbContext dbContext,
            INotificationService notificationService,
            INotificationPublisher notificationPublisher)
        {
            _applicationRepository = applicationRepository;
            _attachmentRepository = attachmentRepository;
            _fileUploadService = fileUploadService;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _notificationService = notificationService;
            _notificationPublisher = notificationPublisher;
        }

        public async Task<ApplicationActionResult> SubmitApplicationAsync(SubmitApplicationRequest request, CancellationToken cancellationToken = default)
        {
            var errors = ValidateSubmitRequest(request);
            if (errors.Count > 0)
            {
                return ApplicationActionResult.Failed(errors);
            }

            var job = await _applicationRepository.GetOpenJobByIdAsync(request.JobId, cancellationToken);
            if (job is null)
            {
                return ApplicationActionResult.Failed("The selected job is not available for applications.");
            }

            var alreadyApplied = await _applicationRepository.HasFreelancerAppliedAsync(request.JobId, request.FreelancerUserId, cancellationToken);
            if (alreadyApplied)
            {
                return ApplicationActionResult.Failed("You have already submitted an application for this job.");
            }

            var uploadedPortfolioFiles = new List<FileUploadResult>();
            var application = new ApplicationEntity
            {
                JobId = request.JobId,
                FreelancerUserId = request.FreelancerUserId,
                ProposedAmount = request.ProposedAmount,
                CoverLetter = request.CoverLetter.Trim(),
                TimelineDays = request.TimelineDays,
                ApplicationStatus = ApplicationStatus.Submitted,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
               
                await _applicationRepository.AddAsync(application, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                foreach (var portfolioFile in request.PortfolioFiles)
                {
                    var upload = await _fileUploadService.UploadPortfolioFileAsync(portfolioFile, PortfolioUploadsFolder, cancellationToken);
                    uploadedPortfolioFiles.Add(upload);

                    var attachment = new Attachment
                    {
                        UploadedByUserId = request.FreelancerUserId,
                        OriginalFileName = upload.OriginalFileName,
                        StoredFileName = upload.StoredFileName,
                        FileUrl = upload.FileUrl,
                        ContentType = upload.ContentType,
                        FileSize = upload.FileSize
                    };

                    // Add Attachment to DB
                    await _attachmentRepository.AddAsync(attachment, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Add join relationship using navigation properties rather than raw IDs
                    application.ApplicationAttachments.Add(new ApplicationAttachment
                    {
                        ApplicationId = application.ApplicationId,
                        AttachmentId = attachment.AttachmentId
                    });
                }

                await _notificationService.CreateAsync(new CreateNotificationRequest
                {
                    RecipientUserId = job.ClientUserId,
                    ActorUserId = request.FreelancerUserId,
                    NotificationType = NotificationType.ApplicationSubmitted,
                    Title = "New application",
                    Message = $"A freelancer applied to {job.Title}.",
                    TargetUrl = $"/Applications/SubmittedApplications?jobId={job.JobId}",
                    RelatedEntityId = application.ApplicationId
                });

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                await _notificationPublisher.NotifyChangedAsync(job.ClientUserId);
                return ApplicationActionResult.Success();
            }
            catch (FileUploadException ex)
            {
                await RollbackAndCleanupUploadsAsync(uploadedPortfolioFiles, cancellationToken);
                return ApplicationActionResult.Failed(ex.Message);
            }
            catch (DbUpdateException)
            {
                await RollbackAndCleanupUploadsAsync(uploadedPortfolioFiles, cancellationToken);
                return ApplicationActionResult.Failed("Unable to submit your application right now. Please try again.");
            }
            catch (IOException)
            {
                await RollbackAndCleanupUploadsAsync(uploadedPortfolioFiles, cancellationToken);
                return ApplicationActionResult.Failed("A file system error occurred while saving your portfolio files. Please try again.");
            }
            catch (InvalidOperationException)
            {
                await RollbackAndCleanupUploadsAsync(uploadedPortfolioFiles, cancellationToken);
                return ApplicationActionResult.Failed("Unable to process your application at this moment. Please try again.");
            }
        }
        public Task<Job?> GetOpenJobByIdAsync(int jobId, CancellationToken cancellationToken = default)
        {
            // Uses the repository method that already checks if job is Open and not deleted
            return _applicationRepository.GetOpenJobByIdAsync(jobId, cancellationToken);
        }
        public async Task<FreelancerApplicationDashboardResult> GetFreelancerDashboardAsync(int freelancerUserId, CancellationToken cancellationToken = default)
        {
            var applications = await _applicationRepository.ListByFreelancerUserIdAsync(freelancerUserId, cancellationToken);

            return new FreelancerApplicationDashboardResult
            {
                Applications = applications.Select(application => new FreelancerApplicationListItemResult
                {
                    ApplicationId = application.ApplicationId,
                    JobId = application.JobId,
                    JobTitle = application.Job.Title,
                    ProposedAmount = application.ProposedAmount,
                    TimelineDays = application.TimelineDays,
                    ApplicationStatus = application.ApplicationStatus,
                    PortfolioItemCount = application.ApplicationAttachments.Count,
                    CreatedAt = application.CreatedAt
                }).ToArray(),
            };
        }

        public async Task<ClientApplicationDashboardResult> GetClientDashboardAsync(int clientUserId, int jobId, CancellationToken cancellationToken = default)
        {
            var applications = await _applicationRepository.ListByClientUserIdAsync(clientUserId, cancellationToken);

            if (jobId > 0)
            {
                applications = applications.Where(app => app.JobId == jobId).ToList();
            }

            return new ClientApplicationDashboardResult
            {
                Applications = applications.Select(application => new ClientApplicationListItemResult
                {
                    ApplicationId = application.ApplicationId,
                    JobId = application.JobId,
                    JobTitle = application.Job.Title,
                    FreelancerUserId = application.FreelancerUserId,
                    FreelancerDisplayName = $"{application.FreelancerUser.FirstName} {application.FreelancerUser.LastName}".Trim(),
                    ProposedAmount = application.ProposedAmount,
                    TimelineDays = application.TimelineDays,
                    ApplicationStatus = application.ApplicationStatus,
                    SubmittedAt = application.CreatedAt
                }).ToArray()
            };
        }
        public async Task<List<FreelanceHub.Domain.Models.Application>> GetApplicationsForJobAsync(int jobId, int clientUserId, CancellationToken cancellationToken = default)
        {
            var job = await _dbContext.Jobs
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.JobId == jobId, cancellationToken);

            if (job == null)
            {
                return new List<FreelanceHub.Domain.Models.Application>();
            }

            if (job.ClientUserId != clientUserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to view applications for this job.");
            }
            return await _applicationRepository.GetApplicationsByJobIdAsync(jobId, cancellationToken);
        }

        public async Task<ApplicationActionResult> UpdateApplicationStatusAsync(
     UpdateApplicationStatusRequest request,
     CancellationToken cancellationToken = default)
        {
            if (!AllowedClientStatuses.Contains(request.ApplicationStatus))
            {
                return ApplicationActionResult.Failed("Invalid status update.");
            }

            var application = await _applicationRepository.GetByIdForClientAsync(request.ApplicationId, request.ClientUserId, cancellationToken);
            if (application is null)
            {
                return ApplicationActionResult.Failed("The selected application was not found.");
            }

            var job = application.Job;
            if (job is null)
            {
                return ApplicationActionResult.Failed(application.JobId, "The job for this application was not found.");
            }

            if (application.ApplicationStatus == request.ApplicationStatus)
            {
                return ApplicationActionResult.Success(application.JobId);
            }

            if (application.ApplicationStatus is ApplicationStatus.Accepted or ApplicationStatus.Rejected)
            {
                return ApplicationActionResult.Failed(application.JobId, "Finalized applications cannot be changed.");
            }

            if (request.ApplicationStatus == ApplicationStatus.Accepted
                && await _dbContext.Contracts.AnyAsync(contract => contract.JobId == application.JobId, cancellationToken))
            {
                return ApplicationActionResult.Failed(application.JobId, "A contract already exists for this job.");
            }

            // Update Application Status
            application.ApplicationStatus = request.ApplicationStatus;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // If status is Accepted, create the Contract record
                if (request.ApplicationStatus == ApplicationStatus.Accepted)
                {

                    // Close the Job
                    job.JobStatus = JobStatus.InProgress;
                    job.UpdatedAt = DateTime.UtcNow;

                    var contract = new Contract
                    {
                        JobId = application.JobId,
                        AcceptedApplicationId = application.ApplicationId,
                        AgreedAmount = application.ProposedAmount,
                        ContractStatus = ContractStatus.Draft,
                        StartDate = DateTime.UtcNow,
                        ExpectedCompletionDate = DateTime.UtcNow.AddDays(application.TimelineDays),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _dbContext.Contracts.AddAsync(contract, cancellationToken);
                }

                var accepted = request.ApplicationStatus == ApplicationStatus.Accepted;
                await _notificationService.CreateAsync(new CreateNotificationRequest
                {
                    RecipientUserId = application.FreelancerUserId,
                    ActorUserId = request.ClientUserId,
                    NotificationType = NotificationType.ApplicationStatusChanged,
                    Title = accepted ? "Application accepted" : "Application status updated",
                    Message = accepted
                        ? $"Your application for {job.Title} was accepted and a draft contract was created."
                        : $"Your application for {job.Title} is now {GetApplicationStatusDisplayName(request.ApplicationStatus)}.",
                    TargetUrl = accepted ? "/Contract" : "/Applications/MyApplications",
                    RelatedEntityId = application.ApplicationId
                });

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                await _notificationPublisher.NotifyChangedAsync(application.FreelancerUserId);
            }
            catch (DbUpdateException)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ApplicationActionResult.Failed(
                    application.JobId,
                    "Unable to update the application status right now. Please try again.");
            }

            return ApplicationActionResult.Success(application.JobId);
        }

        private static string GetApplicationStatusDisplayName(ApplicationStatus status)
        {
            return status == ApplicationStatus.UnderReview ? "under review" : status.ToString().ToLowerInvariant();
        }

        private static List<string> ValidateSubmitRequest(SubmitApplicationRequest request)
        {
            var errors = new List<string>();

            if (request.JobId <= 0)
            {
                errors.Add("A valid job is required.");
            }

            if (request.FreelancerUserId <= 0)
            {
                errors.Add("A valid freelancer account is required.");
            }

            if (request.ProposedAmount < 0.01m || request.ProposedAmount > 9999999999999999.99m)
            {
                errors.Add("Bid amount must be between 0.01 and 9999999999999999.99.");
            }
            else if (decimal.Round(request.ProposedAmount, 2) != request.ProposedAmount)
            {
                errors.Add("Bid amount cannot have more than two decimal places.");
            }

            var trimmedCoverLetter = request.CoverLetter?.Trim() ?? string.Empty;
            if (trimmedCoverLetter.Length is < 20 or > 4000)
            {
                errors.Add("Cover letter must be between 20 and 4000 characters.");
            }

            if (request.TimelineDays is < 1 or > 3650)
            {
                errors.Add("Timeline must be between 1 and 3650 days.");
            }

            if (request.PortfolioFiles.Count > MaxPortfolioFiles)
            {
                errors.Add($"You can upload up to {MaxPortfolioFiles} portfolio files.");
            }

            return errors;
        }

        private async Task RollbackAndCleanupUploadsAsync(IEnumerable<FileUploadResult> uploadedPortfolioFiles, CancellationToken cancellationToken)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            foreach (var uploadedFile in uploadedPortfolioFiles)
            {
                await _fileUploadService.DeleteAsync(uploadedFile.StorageKey);
            }
        }

        public async Task<FreelanceHub.Domain.Models.Application?> GetApplicationByIdAsync(int applicationId, int currentUserId, CancellationToken cancellationToken = default)
        {
            // 1. Fetch application along with its related Job, Freelancer, and Attachments
            var application = await _applicationRepository.GetByIdWithDetailsAsync(applicationId, cancellationToken);

            if (application == null)
            {
                return null;
            }

            // 2. Determine authorization: User must be either the Job Owner (Client) or Proposal Owner (Freelancer)
            bool isJobOwner = application.Job != null && application.Job.ClientUserId == currentUserId;
            bool isProposalOwner = application.FreelancerUserId == currentUserId;

            if (!isJobOwner && !isProposalOwner)
            {
                throw new UnauthorizedAccessException("You are not authorized to view this application.");
            }

            return application;
        }
    }
}
