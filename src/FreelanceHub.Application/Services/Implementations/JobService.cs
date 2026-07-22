using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Application.Services.Implementations
{
    public class JobService : IJobService
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly IFileUploadService _fileUploadService;

        public JobService(ApplicationDbContext dbContext, IFileUploadService fileUploadService)
        {
            _dbContext = dbContext;
            _fileUploadService = fileUploadService;
        }

        public async Task<CreateJobResult> CreateJobAsync(
     CreateJobRequest request,
     CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

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

                _dbContext.Jobs.Add(job);

                // Save once to generate JobId
                await _dbContext.SaveChangesAsync(cancellationToken);

                AssignJobAttributes(request, job);

                await UploadJobFiles(request, job, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return new CreateJobResult
                {
                    Succeeded = true,
                    JobId = job.JobId
                };
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
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
                _dbContext.Attachments.Add(attachment);

                var jobAttachment = new JobAttachment
                {
                    JobId = job.JobId,
                    Attachment = attachment
                };
                _dbContext.JobAttachments.Add(jobAttachment);


            }
        }

        private static IEnumerable<int> ParseIds(string? ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return Enumerable.Empty<int>();

            return ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(int.Parse);
        }



        public async Task<IEnumerable<Job>> GetJobsByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Jobs
                .Where(job => job.ClientUserId == clientId)
                .Include(j => j.JobCategories)
                    .ThenInclude(jc => jc.Category)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                .Include(j => j.JobTags)
                    .ThenInclude(jt => jt.Tag)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Job>> GetAllJOpeningJobsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Jobs.Where(job => job.JobStatus == JobStatus.Open)
                .Include(j => j.JobCategories)
                    .ThenInclude(jc => jc.Category)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                .Include(j => j.JobTags)
                    .ThenInclude(jt => jt.Tag)
                .ToListAsync(cancellationToken);
        }

        private void AssignJobAttributes(CreateJobRequest request, Job job)
        {
            _dbContext.JobCategories.AddRange(
                ParseIds(request.CategoryIds)
                    .Select(id => new JobCategory
                    {
                        JobId = job.JobId,
                        CategoryId = id
                    }));

            _dbContext.JobTags.AddRange(
                ParseIds(request.TagIds)
                    .Select(id => new JobTag
                    {
                        JobId = job.JobId,
                        TagId = id
                    }));

            _dbContext.JobSkills.AddRange(
                ParseIds(request.SkillIds)
                    .Select(id => new JobSkill
                    {
                        JobId = job.JobId,
                        SkillId = id
                    }));
        }
        public async Task<Job?> GetJobByIdAsync(int jobId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Jobs
                .Include(j => j.JobCategories)
                    .ThenInclude(jc => jc.Category)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                .Include(j => j.JobTags)
                    .ThenInclude(jt => jt.Tag)
                .Include(j => j.JobAttachments)
                    .ThenInclude(ja => ja.Attachment)
                .FirstOrDefaultAsync(j => j.JobId == jobId, cancellationToken);
        }
    }
}