using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations.Attachments
{
	public class ApplicationAttachmentConfiguration : IEntityTypeConfiguration<ApplicationAttachment>
	{
		public void Configure(EntityTypeBuilder<ApplicationAttachment> builder)
		{
			builder.ToTable("application_attachments");

			builder.HasKey(aa => new { aa.ApplicationId, aa.AttachmentId });

			builder.Property(aa => aa.ApplicationId).HasColumnName("application_id");
			builder.Property(aa => aa.AttachmentId).HasColumnName("attachment_id");

			builder.HasOne(aa => aa.Application)
				.WithMany(application => application.ApplicationAttachments)
				.HasForeignKey(aa => aa.ApplicationId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(aa => aa.Attachment)
				.WithMany(attachment => attachment.ApplicationAttachments)
				.HasForeignKey(aa => aa.AttachmentId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
