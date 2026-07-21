using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;

namespace FreelanceHub.Application.Services.Abstractions
{
	public interface IProfileService
	{
		Task<UserProfileResult?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

		Task<UpdateOperationResult> UpdateCompanyProfileAsync(
			int userId,
			UpdateCompanyProfileRequest request,
			CancellationToken cancellationToken = default);

		Task<UpdateOperationResult> UpdateFreelancerProfileAsync(
			int userId,
			UpdateFreelancerProfileRequest request,
			CancellationToken cancellationToken = default);

		Task<bool> UpdatePhotoAsync(int userId, UploadedFileRequest profileImage, CancellationToken cancellationToken = default);
	}
}
