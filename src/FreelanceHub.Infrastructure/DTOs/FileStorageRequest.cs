namespace FreelanceHub.Infrastructure.DTOs
{
	public class FileStorageRequest
	{
		public FileStorageRequest(Stream content, string originalFileName, string contentType, long size)
		{
			Content = content;
			OriginalFileName = originalFileName;
			ContentType = contentType;
			Size = size;
		}

		public Stream Content { get; }

		public string OriginalFileName { get; }

		public string ContentType { get; }

		public long Size { get; }
	}
}
