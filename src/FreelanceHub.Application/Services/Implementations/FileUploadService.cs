using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Exceptions;
using FreelanceHub.Application.Services.Abstractions;
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

        private static readonly HashSet<string> AllowedPortfolioContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/webp",
            "image/gif",
            "application/pdf"
        };

        private static readonly HashSet<string> AllowedPortfolioExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".gif",
            ".pdf"
        };

        private const long MaxPortfolioFileSizeBytes = 10 * 1024 * 1024;
        private const long MaxImageSizeBytes = 2 * 1024 * 1024;
        private readonly IFileStorageRepository _fileStorageRepository;

        public FileUploadService(IFileStorageRepository fileStorageRepository)
        {
            _fileStorageRepository = fileStorageRepository;
        }

        public Task<FileUploadResult> UploadPortfolioFileAsync(UploadedFileRequest file, string folderName, CancellationToken cancellationToken = default)
        {
            return UploadFileAsync(
                file,
                folderName,
                AllowedPortfolioExtensions,
                AllowedPortfolioContentTypes,
                MaxPortfolioFileSizeBytes,
                "Portfolio files must be 10 MB or smaller.",
                "Only PDF, JPG, PNG, WEBP, and GIF files are allowed for portfolio uploads.",
                cancellationToken);
        }

        public Task<FileUploadResult> UploadJobFileAsync(UploadedFileRequest file, string folderName, CancellationToken cancellationToken = default)
        {
            return UploadFileAsync(
                file,
                folderName,
                AllowedPortfolioExtensions,
                AllowedPortfolioContentTypes,
                MaxPortfolioFileSizeBytes,
                "Job files must be 10 MB or smaller.",
                "Only PDF, JPG, PNG, WEBP, and GIF files are allowed for job uploads.",
                cancellationToken);
        }

        public Task<FileUploadResult> UploadImageAsync(UploadedFileRequest file, string folderName, CancellationToken cancellationToken = default)
        {
            return UploadFileAsync(
                file,
                folderName,
                AllowedExtensions,
                AllowedContentTypes,
                MaxImageSizeBytes,
                "The profile image must be 2 MB or smaller.",
                "Only JPG, PNG, WEBP, and GIF images are allowed.",
                cancellationToken);
        }

        private async Task<FileUploadResult> UploadFileAsync(
            UploadedFileRequest file,
            string folderName,
            IReadOnlySet<string> allowedExtensions,
            IReadOnlySet<string> allowedContentTypes,
            long maxFileSizeBytes,
            string maxFileSizeErrorMessage,
            string invalidTypeErrorMessage,
            CancellationToken cancellationToken)
        {
            if (file.Size == 0)
            {
                throw new FileUploadException("The uploaded file is empty.");
            }

            if (file.Size > maxFileSizeBytes)
            {
                throw new FileUploadException(maxFileSizeErrorMessage);
            }

            var originalFileName = Path.GetFileName(file.OriginalFileName);
            if (originalFileName.Length > 255)
            {
                throw new FileUploadException("The filename must be 255 characters or fewer.");
            }

            var extension = Path.GetExtension(originalFileName);
            if (!allowedExtensions.Contains(extension)
                || !allowedContentTypes.Contains(file.ContentType)
                || !IsExtensionCompatible(extension, file.ContentType))
            {
                throw new FileUploadException(invalidTypeErrorMessage);
            }

            var result = await _fileStorageRepository.SaveAsync(
                file.Content,
                originalFileName,
                folderName,
                cancellationToken);

            return new FileUploadResult
            {
                OriginalFileName = originalFileName,
                StoredFileName = result.StoredFileName,
                FileUrl = result.FileUrl,
                ContentType = file.ContentType,
                FileSize = file.Size,
                StorageKey = result.StorageKey
            };
        }

        public Task DeleteAsync(string storageKey)
        {
            return _fileStorageRepository.DeleteAsync(storageKey);
        }

        private static bool IsExtensionCompatible(string extension, string contentType)
        {
            return contentType.ToLowerInvariant() switch
            {
                "image/jpeg" => extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
                    || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase),
                "image/png" => extension.Equals(".png", StringComparison.OrdinalIgnoreCase),
                "image/webp" => extension.Equals(".webp", StringComparison.OrdinalIgnoreCase),
                "image/gif" => extension.Equals(".gif", StringComparison.OrdinalIgnoreCase),
                "application/pdf" => extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }
    }
}
