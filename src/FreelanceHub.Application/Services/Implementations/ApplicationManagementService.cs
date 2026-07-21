using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Exceptions;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
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

        public ApplicationManagementService(
            IApplicationRepository applicationRepository,
            IAttachmentRepository attachmentRepository,
            IFileUploadService fileUploadService,
            IUnitOfWork unitOfWork)
        {
            _applicationRepository = applicationRepository;
            _attachmentRepository = attachmentRepository;
            _fileUploadService = fileUploadService;
            _unitOfWork = unitOfWork;
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
                ApplicationStatus = ApplicationStatus.Submitted
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

                    await _attachmentRepository.AddAsync(attachment, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    application.ApplicationAttachments.Add(new ApplicationAttachment
                    {
                        ApplicationId = application.ApplicationId,
                        AttachmentId = attachment.AttachmentId
                    });
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
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

        public async Task<ClientApplicationDashboardResult> GetClientDashboardAsync(int clientUserId, CancellationToken cancellationToken = default)
        {
            var applications = await _applicationRepository.ListByClientUserIdAsync(clientUserId, cancellationToken);

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

        public async Task<ApplicationActionResult> UpdateApplicationStatusAsync(UpdateApplicationStatusRequest request, CancellationToken cancellationToken = default)
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

            if (application.ApplicationStatus == request.ApplicationStatus)
            {
                return ApplicationActionResult.Success();
            }

            if (application.ApplicationStatus is ApplicationStatus.Accepted or ApplicationStatus.Rejected)
            {
                return ApplicationActionResult.Failed("Finalized applications cannot be changed.");
            }


            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            { 
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ApplicationActionResult.Failed("Unable to update the application status right now. Please try again.");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ApplicationActionResult.Success();
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
                await _fileUploadService.DeleteAsync(uploadedFile.StorageKey, cancellationToken);
            }
        }
    }
}
