using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations.Attachments
{
	public class ClientProfileAttachmentConfiguration : IEntityTypeConfiguration<ClientProfileAttachment>
	{
		public void Configure(EntityTypeBuilder<ClientProfileAttachment> builder)
		{
			builder.ToTable("client_profile_attachments");

			builder.HasKey(cpa => new { cpa.ClientProfileId, cpa.AttachmentId });

			builder.Property(cpa => cpa.ClientProfileId).HasColumnName("client_profile_id");
			builder.Property(cpa => cpa.AttachmentId).HasColumnName("attachment_id");
			builder.Property(cpa => cpa.AttachmentType)
				.HasColumnName("attachment_type")
				.HasMaxLength(30)
				.IsRequired();

			builder.HasOne(cpa => cpa.ClientProfile)
				.WithMany(profile => profile.ClientProfileAttachments)
				.HasForeignKey(cpa => cpa.ClientProfileId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(cpa => cpa.Attachment)
				.WithMany(attachment => attachment.ClientProfileAttachments)
				.HasForeignKey(cpa => cpa.AttachmentId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
