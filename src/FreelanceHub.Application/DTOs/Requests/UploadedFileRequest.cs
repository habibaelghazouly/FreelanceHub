namespace FreelanceHub.Application.DTOs.Requests
{
	public class UploadedFileRequest
	{
		public UploadedFileRequest(Stream content, string originalFileName, string contentType, long size)
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
