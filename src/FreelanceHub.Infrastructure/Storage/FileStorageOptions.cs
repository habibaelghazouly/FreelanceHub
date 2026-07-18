namespace FreelanceHub.Infrastructure.Storage
{
	public class FileStorageOptions
	{
		public string RootPath { get; set; } = string.Empty;

		public string PublicBasePath { get; set; } = "uploads";
	}
}
