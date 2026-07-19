using FreelanceHub.Domain.Models;

namespace FreelanceHub.Infrastructure.Repositories.Abstractions
{
	public interface IAttachmentRepository
	{
		Task AddAsync(Attachment attachment, CancellationToken cancellationToken = default);

		Task<Attachment?> GetByIdAsync(int attachmentId, CancellationToken cancellationToken = default);

		Task<bool> IsReferencedAsync(int attachmentId, CancellationToken cancellationToken = default);

		void Remove(Attachment attachment);
	}
}
