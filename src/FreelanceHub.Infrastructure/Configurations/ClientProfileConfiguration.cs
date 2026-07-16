using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
	public class ClientProfileConfiguration : IEntityTypeConfiguration<ClientProfile>
	{
		public void Configure(EntityTypeBuilder<ClientProfile> builder)
		{
			builder.ToTable("client_profiles");
			builder.HasKey(profile => profile.ClientProfileId);

			builder.Property(profile => profile.ClientProfileId).HasColumnName("client_profile_id");
			builder.Property(profile => profile.UserId).HasColumnName("user_id");
			builder.Property(profile => profile.CompanyName).HasColumnName("company_name").HasMaxLength(150);
			builder.Property(profile => profile.CompanyDescription).HasColumnName("company_description");
			builder.Property(profile => profile.CompanyWebsite).HasColumnName("company_website").HasMaxLength(500);
			builder.Property(profile => profile.CompanyLogoAttachmentId).HasColumnName("company_logo_attachment_id");
			builder.Property(profile => profile.RatingAverage).HasColumnName("rating_averge").HasDefaultValue(0);
			builder.Property(profile => profile.RatingCount).HasColumnName("rating_count").HasDefaultValue(0);
			builder.Property(profile => profile.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSDATETIME()");
			builder.Property(profile => profile.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("SYSDATETIME()");

			builder.HasIndex(profile => profile.UserId).IsUnique();

			builder
				.HasOne(profile => profile.User)
				.WithOne(user => user.ClientProfile)
				.HasForeignKey<ClientProfile>(profile => profile.UserId)
				.OnDelete(DeleteBehavior.NoAction);

			builder
				.HasOne(profile => profile.CompanyLogoAttachment)
				.WithMany()
				.HasForeignKey(profile => profile.CompanyLogoAttachmentId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
