using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

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

		public Task<Attachment?> GetByIdAsync(int attachmentId, CancellationToken cancellationToken = default)
		{
			return _dbContext.Attachments.SingleOrDefaultAsync(
				attachment => attachment.AttachmentId == attachmentId,
				cancellationToken);
		}

		public async Task<bool> IsReferencedAsync(int attachmentId, CancellationToken cancellationToken = default)
		{
			if (await _dbContext.Users
				.IgnoreQueryFilters()
				.AnyAsync(user => user.ProfileImageAttachmentId == attachmentId, cancellationToken))
			{
				return true;
			}

			if (await _dbContext.ClientProfiles
				.IgnoreQueryFilters()
				.AnyAsync(profile => profile.CompanyLogoAttachmentId == attachmentId, cancellationToken))
			{
				return true;
			}

			return await _dbContext.Attachments
				.IgnoreQueryFilters()
				.AnyAsync(attachment => attachment.AttachmentId == attachmentId
					&& (attachment.FreelancerProfileAttachments.Any()
						|| attachment.ClientProfileAttachments.Any()
						|| attachment.JobAttachments.Any()
						|| attachment.ApplicationAttachments.Any()
						|| attachment.ContractAttachments.Any()), cancellationToken);
		}

		public void Remove(Attachment attachment)
		{
			_dbContext.Attachments.Remove(attachment);
		}
	}
}
