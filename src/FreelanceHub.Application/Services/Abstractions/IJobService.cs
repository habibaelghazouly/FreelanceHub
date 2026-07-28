using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Domain.Models;

namespace FreelanceHub.Application.Services.Abstractions
{

    public interface IJobService
    {
        Task<CreateJobResult> CreateJobAsync(CreateJobRequest request, CancellationToken cancellationToken = default);

        Task<Job?> GetJobByIdAsync(int jobId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Job>> GetAllJOpeningJobsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Job>> GetJobsByClientIdAsync(int clientId, CancellationToken cancellationToken = default);

        Task<CreateJobPageResult> GetCreateJobPageDataAsync(CancellationToken cancellationToken = default);

        Task<BrowseJobsResult> BrowseJobsAsync(
    int? categoryId,
    int? skillId,
    decimal? maxBudget,
    string? sortOrder,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken = default);
    }



}
