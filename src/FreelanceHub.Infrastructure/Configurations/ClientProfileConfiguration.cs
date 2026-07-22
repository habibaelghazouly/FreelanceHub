using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
	public class ClientProfileConfiguration : IEntityTypeConfiguration<ClientProfile>
	{
		public void Configure(EntityTypeBuilder<ClientProfile> builder)
		{
			builder.ToTable("client_profiles", table =>
			{
				table.HasCheckConstraint("chk_client_profiles_type", "[client_type] IN (70, 71)");
				table.HasCheckConstraint(
					"chk_client_profiles_company_details",
					"[client_type] = 70 OR ([client_type] = 71 AND NULLIF(LTRIM(RTRIM([company_name])), '') IS NOT NULL AND NULLIF(LTRIM(RTRIM([company_description])), '') IS NOT NULL)");
			});
			builder.HasKey(profile => profile.ClientProfileId);

			builder.Property(profile => profile.ClientProfileId).HasColumnName("client_profile_id");
			builder.Property(profile => profile.UserId).HasColumnName("user_id");
			builder.Property(profile => profile.ClientType).HasColumnName("client_type").HasConversion<int>().IsRequired();
			builder.Property(profile => profile.CompanyName).HasColumnName("company_name").HasMaxLength(150);
			builder.Property(profile => profile.CompanyDescription).HasColumnName("company_description").HasMaxLength(2000);
			builder.Property(profile => profile.CompanyWebsite).HasColumnName("company_website").HasMaxLength(500);
			builder.Property(profile => profile.CompanyLogoAttachmentId).HasColumnName("company_logo_attachment_id");
			builder.Property(profile => profile.RatingAverage).HasColumnName("rating_averge").HasPrecision(3, 2).HasDefaultValue(0m);
			builder.Property(profile => profile.RatingCount).HasColumnName("rating_count").HasDefaultValue(0);
			builder.Property(profile => profile.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSUTCDATETIME()");
			builder.Property(profile => profile.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("SYSUTCDATETIME()");

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
