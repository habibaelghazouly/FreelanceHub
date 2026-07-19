using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations.Attachments
{
	public class FreelancerProfileAttachmentConfiguration : IEntityTypeConfiguration<FreelancerProfileAttachment>
	{
		public void Configure(EntityTypeBuilder<FreelancerProfileAttachment> builder)
		{
			builder.ToTable("freelancer_profile_attachments");

			builder.HasKey(fpa => new { fpa.FreelancerProfileId, fpa.AttachmentId });

			builder.Property(fpa => fpa.FreelancerProfileId).HasColumnName("freelancer_profile_id");
			builder.Property(fpa => fpa.AttachmentId).HasColumnName("attachment_id");
			builder.Property(fpa => fpa.AttachmentDescription)
				.HasColumnName("attachment_description")
				.HasMaxLength(30)
				.IsRequired();

			builder.HasOne(fpa => fpa.FreelancerProfile)
				.WithMany(profile => profile.FreelancerProfileAttachments)
				.HasForeignKey(fpa => fpa.FreelancerProfileId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(fpa => fpa.Attachment)
				.WithMany(attachment => attachment.FreelancerProfileAttachments)
				.HasForeignKey(fpa => fpa.AttachmentId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
