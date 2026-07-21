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

            var extension = Path.GetExtension(file.OriginalFileName);
            if (!allowedExtensions.Contains(extension) || !allowedContentTypes.Contains(file.ContentType))
            {
                throw new FileUploadException(invalidTypeErrorMessage);
            }

            var result = await _fileStorageRepository.SaveAsync(
                file.Content,
                file.OriginalFileName,
                folderName,
                cancellationToken);

            return new FileUploadResult
            {
                OriginalFileName = Path.GetFileName(file.OriginalFileName),
                StoredFileName = result.StoredFileName,
                FileUrl = result.FileUrl,
                ContentType = file.ContentType,
                FileSize = file.Size,
                StorageKey = result.StorageKey
            };
        }

        public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
        {
            return _fileStorageRepository.DeleteAsync(storageKey, cancellationToken);
        }
    }

}
