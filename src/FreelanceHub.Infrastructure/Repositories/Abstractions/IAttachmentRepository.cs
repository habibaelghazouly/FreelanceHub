using FreelanceHub.Domain.Models;

namespace FreelanceHub.Infrastructure.Repositories.Abstractions
{
	public interface IAttachmentRepository
	{
		Task AddAsync(Attachment attachment, CancellationToken cancellationToken = default);
	}
}
