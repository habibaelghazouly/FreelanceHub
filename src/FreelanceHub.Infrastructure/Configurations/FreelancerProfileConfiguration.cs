using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
	public class FreelancerProfileConfiguration : IEntityTypeConfiguration<FreelancerProfile>
	{
		public void Configure(EntityTypeBuilder<FreelancerProfile> builder)
		{
			builder.ToTable("freelancer_profiles", table =>
			{
				table.HasCheckConstraint("chk_freelancer_profiles_experience_level", "[experience_level] IN (30, 31, 32)");
				table.HasCheckConstraint("chk_freelancer_profiles_availability_status", "[availability_status] IN (20, 21, 22)");
				table.HasCheckConstraint("chk_freelancer_profiles_hourly_rate", "[hourly_rate] > 0");
				table.HasCheckConstraint(
					"chk_freelancer_profiles_required_details",
					"NULLIF(LTRIM(RTRIM([professional_title])), '') IS NOT NULL AND LEN(LTRIM(RTRIM([bio]))) >= 20");
			});
			builder.HasKey(profile => profile.FreelancerProfileId);

			builder.Property(profile => profile.FreelancerProfileId).HasColumnName("freelancer_profile_id");
			builder.Property(profile => profile.UserId).HasColumnName("user_id");
			builder.Property(profile => profile.ProfessionalTitle).HasColumnName("professional_title").HasMaxLength(150).IsRequired();
			builder.Property(profile => profile.HourlyRate).HasColumnName("hourly_rate").HasPrecision(18, 2).IsRequired();
			builder.Property(profile => profile.Bio).HasColumnName("bio").HasMaxLength(2000).IsRequired();
			builder.Property(profile => profile.ExperienceLevel).HasColumnName("experience_level").HasConversion<int>().IsRequired();
			builder.Property(profile => profile.AvailabilityStatus).HasColumnName("availability_status").HasConversion<int>().IsRequired();
			builder.Property(profile => profile.ExternalPortfolioUrl).HasColumnName("external_portfolio_url").HasMaxLength(500);
			builder.Property(profile => profile.RatingAverage).HasColumnName("rating_averge").HasPrecision(3, 2).HasDefaultValue(0m);
			builder.Property(profile => profile.RatingCount).HasColumnName("rating_count").HasDefaultValue(0);
			builder.Property(profile => profile.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSUTCDATETIME()");
			builder.Property(profile => profile.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("SYSUTCDATETIME()");

			builder.HasIndex(profile => profile.UserId).IsUnique();

			builder
				.HasOne(profile => profile.User)
				.WithOne(user => user.FreelancerProfile)
				.HasForeignKey<FreelancerProfile>(profile => profile.UserId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
