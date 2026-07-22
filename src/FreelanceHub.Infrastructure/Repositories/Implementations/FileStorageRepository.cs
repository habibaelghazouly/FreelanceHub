using FreelanceHub.Infrastructure.Repositories.Abstractions;
using FreelanceHub.Infrastructure.Storage;

namespace FreelanceHub.Infrastructure.Repositories.Implementations
{
	public class FileStorageRepository : IFileStorageRepository
	{
		private readonly FileStorageOptions _options;

		public FileStorageRepository(FileStorageOptions options)
		{
			_options = options;
		}

		public async Task<(string StoredFileName, string FileUrl, string StorageKey)> SaveAsync(
			Stream content,
			string originalFileName,
			string folderName,
			CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(_options.RootPath))
			{
				throw new InvalidOperationException("File storage root path is not configured.");
			}

			var safeFolderName = NormalizeFolderName(folderName);
			var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
			var storedFileName = $"{Guid.NewGuid():N}{extension}";
			var publicBasePath = NormalizeFolderName(_options.PublicBasePath);
			var storageFolder = string.IsNullOrWhiteSpace(publicBasePath)
				? safeFolderName
				: $"{publicBasePath}/{safeFolderName}";
			var storageKey = $"{storageFolder}/{storedFileName}";
			var physicalFolder = GetSafePhysicalPath(storageFolder);

			Directory.CreateDirectory(physicalFolder);

			var physicalPath = GetSafePhysicalPath(storageKey);
			if (content.CanSeek)
			{
				content.Position = 0;
			}

			try
			{
				await using var stream = File.Create(physicalPath);
				await content.CopyToAsync(stream, cancellationToken);
			}
			catch
			{
				try
				{
					File.Delete(physicalPath);
				}
				catch
				{
					// Preserve the original upload failure if cleanup also fails.
				}

				throw;
			}

			return (storedFileName, $"/{storageKey}", storageKey);
		}

		public Task DeleteAsync(string storageKey)
		{
			if (!string.IsNullOrWhiteSpace(storageKey))
			{
				var physicalPath = GetSafePhysicalPath(storageKey);
				if (File.Exists(physicalPath))
				{
					File.Delete(physicalPath);
				}
			}

			return Task.CompletedTask;
		}

		private string GetSafePhysicalPath(string storageKey)
		{
			var rootPath = Path.GetFullPath(_options.RootPath);
			var physicalPath = Path.GetFullPath(Path.Combine(rootPath, storageKey.Replace('/', Path.DirectorySeparatorChar)));

			if (!physicalPath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Invalid file storage path.");
			}

			return physicalPath;
		}

		private static string NormalizeFolderName(string folderName)
		{
			var normalized = folderName.Replace('\\', '/').Trim('/');
			if (string.IsNullOrWhiteSpace(normalized) || normalized.Contains("..", StringComparison.Ordinal))
			{
				throw new InvalidOperationException("Invalid upload folder name.");
			}

			return normalized;
		}
	}
}
