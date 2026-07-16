namespace FreelanceHub.Application.DTOs.Results
{
	public class FileUploadResult
	{
		public string OriginalFileName { get; set; } = string.Empty;

		public string StoredFileName { get; set; } = string.Empty;

		public string FileUrl { get; set; } = string.Empty;

		public string ContentType { get; set; } = string.Empty;

		public long FileSize { get; set; }

		public string StorageKey { get; set; } = string.Empty;
	}
}
