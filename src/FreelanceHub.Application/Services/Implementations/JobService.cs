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

        private readonly IJobRepository _jobRepository;

        private readonly IAttachmentRepository _attachmentRepository;

        private readonly ILookupRepository _lookupRepository;
        private readonly IFileUploadService _fileUploadService;

        public JobService(IJobRepository jobRepository, IAttachmentRepository attachmentRepository, ILookupRepository lookupRepository, IFileUploadService fileUploadService)
        {
            _jobRepository = jobRepository;
            _attachmentRepository = attachmentRepository;
            _lookupRepository = lookupRepository;
            _fileUploadService = fileUploadService;
        }

        public async Task<CreateJobResult> CreateJobAsync(
    CreateJobRequest request,
    CancellationToken cancellationToken = default)
        {
            await _jobRepository.BeginTransactionAsync(cancellationToken);

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

                await _jobRepository.CreateJobAsync(job, cancellationToken);

                await AssignJobAttributes(request, job, cancellationToken);

                await UploadJobFiles(request, job, cancellationToken);

                await _jobRepository.SaveChangesAsync(cancellationToken);

                await _jobRepository.CommitTransactionAsync(cancellationToken);

                return new CreateJobResult
                {
                    Succeeded = true,
                    JobId = job.JobId
                };
            }
            catch
            {
                await _jobRepository.RollbackTransactionAsync(cancellationToken);
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
                await _jobRepository.AddAttachmentsAsync(new List<JobAttachment> { jobAttachment }, cancellationToken);


            }
        }


        public async Task<IEnumerable<Job>> GetJobsByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
        {
            return await _jobRepository.GetJobsByClientIdAsync(clientId, cancellationToken);
        }

        public async Task<IEnumerable<Job>> GetAllJOpeningJobsAsync(CancellationToken cancellationToken = default)
        {
            return await _jobRepository.GetAllOpeningJobsAsync(cancellationToken);
        }

        private async Task AssignJobAttributes(CreateJobRequest request, Job job, CancellationToken cancellationToken = default)
        {
            await _jobRepository.AddJobCategoriesAsync(request.CategoryIds.Select(id => new JobCategory
            {
                JobId = job.JobId,
                CategoryId = id
            }), cancellationToken);

            await _jobRepository.AddJobTagsAsync(request.TagIds.Select(id => new JobTag
            {
                JobId = job.JobId,
                TagId = id
            }), cancellationToken);

            await _jobRepository.AddJobSkillsAsync(request.SkillIds.Select(id => new JobSkill
            {
                JobId = job.JobId,
                SkillId = id
            }), cancellationToken);
        }
        public async Task<Job?> GetJobByIdAsync(int jobId, CancellationToken cancellationToken = default)
        {
            return await _jobRepository.GetJobByIdAsync(jobId, cancellationToken);
        }

        public async Task<CreateJobPageResult> GetCreateJobPageDataAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _lookupRepository.GetCategoriesAsync(cancellationToken);
            var tags = await _lookupRepository.GetTagsAsync(cancellationToken);
            var skills = await _lookupRepository.GetSkillsAsync(cancellationToken);

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