using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;

namespace FreelanceHub.Application.Services.Implementations
{
    public class JobService : IJobService
    {

        private readonly ApplicationDbContext _dbContext;

        public JobService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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

        private void AssignJobAttributes(CreateJobRequest request, Job job)
        {

            _dbContext.JobCategories.AddRange(
                request.CategoryIds.Split(',').Select(id => new JobCategory
                {
                    JobId = job.JobId,
                    CategoryId = int.Parse(id)
                }));

            _dbContext.JobTags.AddRange(
                request.TagIds.Split(',').Select(id => new JobTag
                {
                    JobId = job.JobId,
                    TagId = int.Parse(id)
                }));

            _dbContext.JobSkills.AddRange(
                request.SkillIds.Split(',').Select(id => new JobSkill
                {
                    JobId = job.JobId,
                    SkillId = int.Parse(id)
                }));
        }
    }
}