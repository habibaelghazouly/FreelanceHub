using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.Repositories.Abstractions;

namespace FreelanceHub.Application.Services.Implementations
{
    public class JobService : IJobService
    {

        private readonly IFileUploadService _fileUploadService;
        private readonly IJobRepository _jobRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public JobService(IJobRepository jobRepository, IAttachmentRepository attachmentRepository, IUnitOfWork unitOfWork, IFileUploadService fileUploadService)
        {
            _jobRepository = jobRepository;
            _attachmentRepository = attachmentRepository;
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
        }

        public async Task<CreateJobResult> CreateJobAsync(
    CreateJobRequest request,
    CancellationToken cancellationToken = default)
        {
            var validationErrors = await ValidateRequestAsync(request, cancellationToken);
            if (validationErrors.Count > 0)
            {
                return new CreateJobResult { Errors = validationErrors };
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var job = new Job
                {
                    Title = request.Title,
                    Description = request.Description,
                    Budget = request.Budget,
                    Deadline = request.Deadline,
                    ClientUserId = request.ClientId
                };

                await _jobRepository.AddAsync(job, cancellationToken);

                // Save once to generate JobId
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await AssignJobAttributes(request, job, cancellationToken);

                await UploadJobFiles(request, job, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new CreateJobResult
                {
                    Succeeded = true,
                    JobId = job.JobId
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        private async Task UploadJobFiles(CreateJobRequest request, Job job, CancellationToken cancellationToken)
        {
            foreach (var file in request.JobFiles)
            {
                var uploadedFile = await _fileUploadService.UploadJobFileAsync(file, "job-files", cancellationToken);
                var attachment = new Attachment
                {
                    UploadedByUserId = request.ClientId,
                    OriginalFileName = uploadedFile.OriginalFileName,
                    StoredFileName = uploadedFile.StoredFileName,
                    FileUrl = uploadedFile.FileUrl,
                    ContentType = uploadedFile.ContentType,
                    FileSize = uploadedFile.FileSize
                };
                await _attachmentRepository.AddAsync(attachment, cancellationToken);

                var jobAttachment = new JobAttachment
                {
                    JobId = job.JobId,
                    Attachment = attachment
                };
                _jobRepository.AddAttachment(jobAttachment);


            }
        }

        private static IReadOnlyList<int> ParseIds(string? ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return Array.Empty<int>();

            return ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(value => int.TryParse(value, out var id) ? id : 0)
                .Where(id => id > 0)
                .Distinct()
                .ToArray();
        }



        public async Task<IEnumerable<Job>> GetJobsByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
        {
            return await _jobRepository.ListByClientIdAsync(clientId, cancellationToken);
        }

        public async Task<IEnumerable<Job>> GetAllJOpeningJobsAsync(CancellationToken cancellationToken = default)
        {
            return await _jobRepository.ListOpenAsync(cancellationToken);
        }

        private async Task AssignJobAttributes(CreateJobRequest request, Job job, CancellationToken cancellationToken = default)
        {
            _jobRepository.AddAttributes(job, ParseIds(request.CategoryIds), ParseIds(request.SkillIds), ParseIds(request.TagIds));
        }
        public async Task<Job?> GetJobByIdAsync(int jobId, CancellationToken cancellationToken = default)
        {
            return await _jobRepository.GetByIdAsync(jobId, cancellationToken);
        }

        private async Task<List<string>> ValidateRequestAsync(CreateJobRequest request, CancellationToken cancellationToken)
        {
            var errors = new List<string>();
            if (request.ClientId <= 0) errors.Add("A valid client is required.");
            if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Trim().Length is < 5 or > 100) errors.Add("Job title must be between 5 and 100 characters.");
            if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Trim().Length is < 20 or > 4000) errors.Add("Job description must be between 20 and 4000 characters.");
            if (request.Budget <= 0) errors.Add("Budget must be greater than zero.");
            if (request.Deadline.Date <= DateTime.UtcNow.Date) errors.Add("Deadline must be in the future.");
            if (request.JobFiles.Count > 10 || request.JobFiles.Any(file => file.Size is <= 0 or > 10 * 1024 * 1024)) errors.Add("Upload no more than 10 files, each up to 10 MB.");
            var allowedContentTypes = new[] { "application/pdf", "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (request.JobFiles.Any(file => !allowedContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))) errors.Add("Only PDF and image files are allowed.");

            var categoryIds = ParseIds(request.CategoryIds);
            var skillIds = ParseIds(request.SkillIds);
            var tagIds = ParseIds(request.TagIds);
            if (!await _jobRepository.AreValidAttributesAsync(categoryIds, skillIds, tagIds, cancellationToken)) errors.Add("One or more selected categories, skills, or tags are invalid.");
            return errors;
        }
        public async Task<CreateJobPageResult> GetCreateJobPageDataAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _jobRepository.ListCategoriesAsync(cancellationToken);
            var tags = await _jobRepository.ListTagsAsync(cancellationToken);
            var skills = await _jobRepository.ListSkillsAsync(cancellationToken);

            return new CreateJobPageResult
            {
                Categories = categories.Select(c => new SelectableItemResult
                {
                    Id = c.CategoryId,
                    Name = c.Name
                }).ToList(),

                Tags = tags.Select(t => new SelectableItemResult
                {
                    Id = t.TagId,
                    Name = t.Name
                }).ToList(),

                Skills = skills.Select(s => new SelectableItemResult
                {
                    Id = s.SkillId,
                    Name = s.Name
                }).ToList()
            };
        }

        public async Task<BrowseJobsResult> BrowseJobsAsync(
            int? categoryId,
            int? skillId,
            decimal? maxBudget,
            string? sortOrder,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var (jobs, totalCount) = await _jobRepository.BrowseJobsAsync(categoryId, skillId, maxBudget, sortOrder, pageNumber, pageSize, cancellationToken);

            return new BrowseJobsResult
            {
                Jobs = jobs,
                TotalCount = totalCount
            };
        }
    }

}
