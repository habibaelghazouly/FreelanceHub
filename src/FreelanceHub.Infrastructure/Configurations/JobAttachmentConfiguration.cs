using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations.Attachments
{
	public class JobAttachmentConfiguration : IEntityTypeConfiguration<JobAttachment>
	{
		public void Configure(EntityTypeBuilder<JobAttachment> builder)
		{
			builder.ToTable("job_attachments");

			builder.HasKey(ja => new { ja.JobId, ja.AttachmentId });

			builder.Property(ja => ja.JobId).HasColumnName("job_id");
			builder.Property(ja => ja.AttachmentId).HasColumnName("attachment_id");

			builder.HasQueryFilter(ja => !ja.Job.IsDeleted);

			builder.HasOne(ja => ja.Job)
				.WithMany(job => job.JobAttachments)
				.HasForeignKey(ja => ja.JobId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(ja => ja.Attachment)
				.WithMany(attachment => attachment.JobAttachments)
				.HasForeignKey(ja => ja.AttachmentId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
