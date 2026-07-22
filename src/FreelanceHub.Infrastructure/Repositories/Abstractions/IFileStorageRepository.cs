namespace FreelanceHub.Infrastructure.Repositories.Abstractions
{
	public interface IFileStorageRepository
	{
		Task<(string StoredFileName, string FileUrl, string StorageKey)> SaveAsync(
			Stream content,
			string originalFileName,
			string folderName,
			CancellationToken cancellationToken = default);

		Task DeleteAsync(string storageKey);
	}
}
