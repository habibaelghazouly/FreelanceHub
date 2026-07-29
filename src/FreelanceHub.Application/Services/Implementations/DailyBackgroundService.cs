using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Infrastructure.Repositories.Abstractions;

namespace FreelanceHub.Application.Services.Implementations
{
    public class DailyBackgroundService : IDailyBackgroundService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IContractRepository _contractRepository;

        private readonly IApplicationRepository _applicationRepository;



        public DailyBackgroundService(IJobRepository jobRepository, IContractRepository contractRepository, IApplicationRepository applicationRepository)
        {
            _jobRepository = jobRepository;
            _contractRepository = contractRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task ExecuteDailyTasksAsync()
        {
            
            await UpdateJobStatusToOverdue();

            await UpdateContractStatusToOverdue();
        }

        private async Task UpdateJobStatusToOverdue()
        {
            // Get all jobs that have passed their deadline
            var expiredJobs = await _jobRepository.GetExpiredJobsAsync();
            foreach (var job in expiredJobs)
            {
                // Update the status of the job to "Closed"
                job.JobStatus = JobStatus.Overdue;
                await _jobRepository.UpdateJobAsync(job);

                // Get all applications for the expired job
                var applications = await _applicationRepository.GetApplicationsByJobIdAsync(job.JobId);

                foreach (var application in applications)
                {
                    // Update the status of each application to "Rejected"
                    application.ApplicationStatus = ApplicationStatus.Rejected;
                    await _applicationRepository.UpdateApplicationAsync(application);
                }
            }
        }

        private async Task UpdateContractStatusToOverdue()
        {
            var expiredContracts = await _contractRepository.GetExpiredContractsAsync();

            foreach (var contract in expiredContracts)
            {
                // Update the status of the contract to "Completed"
                contract.ContractStatus = ContractStatus.Overdue;
                await _contractRepository.UpdateContractAsync(contract);
            }
            await _applicationRepository.SaveChangesAsync();
        }
    }
}