using FreelanceHub.Infrastructure.DTOs;

namespace FreelanceHub.Infrastructure.Repositories.Abstractions
{
	public interface IFileStorageRepository
	{
		Task<FileStorageResult> SaveAsync(FileStorageRequest file, string folderName, CancellationToken cancellationToken = default);

		Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
	}
}
