using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Exceptions;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Infrastructure.DTOs;
using FreelanceHub.Infrastructure.Repositories.Abstractions;

namespace FreelanceHub.Application.Services.Implementations
{
	public class FileUploadService : IFileUploadService
	{
		private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
		{
			"image/jpeg",
			"image/png",
			"image/webp",
			"image/gif"
		};

		private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
		{
			".jpg",
			".jpeg",
			".png",
			".webp",
			".gif"
		};

		private const long MaxImageSizeBytes = 2 * 1024 * 1024;
		private readonly IFileStorageRepository _fileStorageRepository;

		public FileUploadService(IFileStorageRepository fileStorageRepository)
		{
			_fileStorageRepository = fileStorageRepository;
		}

		public async Task<FileUploadResult> UploadImageAsync(UploadedFileRequest file, string folderName, CancellationToken cancellationToken = default)
		{
			if (file.Size == 0)
			{
				throw new FileUploadException("The uploaded image is empty.");
			}

			if (file.Size > MaxImageSizeBytes)
			{
				throw new FileUploadException("The profile image must be 2 MB or smaller.");
			}

			var extension = Path.GetExtension(file.OriginalFileName);
			if (!AllowedExtensions.Contains(extension) || !AllowedContentTypes.Contains(file.ContentType))
			{
				throw new FileUploadException("Only JPG, PNG, WEBP, and GIF images are allowed.");
			}

			var result = await _fileStorageRepository.SaveAsync(new FileStorageRequest(
				file.Content,
				file.OriginalFileName,
				file.ContentType,
				file.Size), folderName, cancellationToken);

			return new FileUploadResult
			{
				OriginalFileName = result.OriginalFileName,
				StoredFileName = result.StoredFileName,
				FileUrl = result.FileUrl,
				ContentType = result.ContentType,
				FileSize = result.FileSize,
				StorageKey = result.StorageKey
			};
		}

		public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
		{
			return _fileStorageRepository.DeleteAsync(storageKey, cancellationToken);
		}
	}
}
