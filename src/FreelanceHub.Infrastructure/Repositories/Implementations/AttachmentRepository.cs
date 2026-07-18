using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;

namespace FreelanceHub.Infrastructure.Repositories.Implementations
{
	public class AttachmentRepository : IAttachmentRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public AttachmentRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(Attachment attachment, CancellationToken cancellationToken = default)
		{
			await _dbContext.Attachments.AddAsync(attachment, cancellationToken);
		}
	}
}
