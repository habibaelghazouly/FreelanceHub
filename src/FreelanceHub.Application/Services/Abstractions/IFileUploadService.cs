using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;

namespace FreelanceHub.Application.Services.Abstractions
{
	public interface IFileUploadService
	{
		Task<FileUploadResult> UploadImageAsync(UploadedFileRequest file, string folderName, CancellationToken cancellationToken = default);

		Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
	}
}
