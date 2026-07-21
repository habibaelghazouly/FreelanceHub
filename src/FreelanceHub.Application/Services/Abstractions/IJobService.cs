using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;

namespace FreelanceHub.Application.Services.Abstractions
{
    
    public interface IJobService
    {
        Task<CreateJobResult> CreateJobAsync(CreateJobRequest request, CancellationToken cancellationToken = default);
    }
}