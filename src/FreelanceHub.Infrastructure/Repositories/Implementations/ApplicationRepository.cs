using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Repositories.Implementations
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ApplicationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Job?> GetOpenJobByIdAsync(int jobId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Jobs
                .AsNoTracking()
                .SingleOrDefaultAsync(job =>
                    job.JobId == jobId
                    && job.JobStatus == JobStatus.Open
                    && !job.IsDeleted, cancellationToken);
        }

        public Task<bool> HasFreelancerAppliedAsync(int jobId, int freelancerUserId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Applications
                .AsNoTracking()
                .AnyAsync(application =>
                    application.JobId == jobId
                    && application.FreelancerUserId == freelancerUserId, cancellationToken);
        }

        public async Task AddAsync(Application application, CancellationToken cancellationToken = default)
        {
            await _dbContext.Applications.AddAsync(application, cancellationToken);
        }

        public Task<List<Application>> ListByFreelancerUserIdAsync(int freelancerUserId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Applications
                .AsNoTracking()
                .Where(application => application.FreelancerUserId == freelancerUserId)
                .Include(application => application.Job)
                .Include(application => application.ApplicationAttachments)
                .OrderByDescending(application => application.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public Task<List<Application>> ListByClientUserIdAsync(int clientUserId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Applications
                .AsNoTracking()
                .Where(application => application.Job.ClientUserId == clientUserId)
                .Include(application => application.Job)
                .Include(application => application.FreelancerUser)
                .OrderByDescending(application => application.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public Task<Application?> GetByIdForClientAsync(int applicationId, int clientUserId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Applications
                .Include(application => application.Job)
                .Include(application => application.FreelancerUser)
                .SingleOrDefaultAsync(application =>
                    application.ApplicationId == applicationId
                    && application.Job.ClientUserId == clientUserId, cancellationToken);
        }
        public async Task<List<Application>> GetApplicationsByJobIdAsync(int jobId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Applications
                .Include(a => a.FreelancerUser)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
