using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
	public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
	{
		public void Configure(EntityTypeBuilder<Attachment> builder)
		{
			builder.ToTable("attachments", table =>
				table.HasCheckConstraint("chk_attachments_file_size", "[file_size] IS NULL OR [file_size] >= 0"));
			builder.HasKey(attachment => attachment.AttachmentId);

			builder.Property(attachment => attachment.AttachmentId).HasColumnName("attachment_id");
			builder.Property(attachment => attachment.UploadedByUserId).HasColumnName("uploaded_by_user_id");
			builder.Property(attachment => attachment.OriginalFileName).HasColumnName("original_file_name").HasMaxLength(255).IsRequired();
			builder.Property(attachment => attachment.StoredFileName).HasColumnName("stored_file_name").HasMaxLength(255);
			builder.Property(attachment => attachment.FileUrl).HasColumnName("file_url").HasMaxLength(500).IsRequired();
			builder.Property(attachment => attachment.ContentType).HasColumnName("content_type").HasMaxLength(100);
			builder.Property(attachment => attachment.FileSize).HasColumnName("file_size");
			builder.Property(attachment => attachment.UploadedAt).HasColumnName("uploaded_at").HasDefaultValueSql("SYSDATETIME()");

			builder
				.HasOne(attachment => attachment.UploadedByUser)
				.WithMany(user => user.UploadedAttachments)
				.HasForeignKey(attachment => attachment.UploadedByUserId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
