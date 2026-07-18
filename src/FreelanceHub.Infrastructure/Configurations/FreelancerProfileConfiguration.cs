using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
	public class FreelancerProfileConfiguration : IEntityTypeConfiguration<FreelancerProfile>
	{
		public void Configure(EntityTypeBuilder<FreelancerProfile> builder)
		{
			builder.ToTable("freelancer_profiles");
			builder.HasKey(profile => profile.FreelancerProfileId);

			builder.Property(profile => profile.FreelancerProfileId).HasColumnName("freelancer_profile_id");
			builder.Property(profile => profile.UserId).HasColumnName("user_id");
			builder.Property(profile => profile.ProfessionalTitle).HasColumnName("professional_title").HasMaxLength(150);
			builder.Property(profile => profile.HourlyRate).HasColumnName("hourly_rate").HasPrecision(18, 2);
			builder.Property(profile => profile.Bio).HasColumnName("bio");
			builder.Property(profile => profile.ExperienceLevel).HasColumnName("experience_level").HasMaxLength(30);
			builder.Property(profile => profile.AvailabilityStatus).HasColumnName("availability_status");
			builder.Property(profile => profile.ExternalPortfolioUrl).HasColumnName("external_portfolio_url").HasMaxLength(500);
			builder.Property(profile => profile.RatingAverage).HasColumnName("rating_averge").HasDefaultValue(0);
			builder.Property(profile => profile.RatingCount).HasColumnName("rating_count").HasDefaultValue(0);
			builder.Property(profile => profile.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSDATETIME()");
			builder.Property(profile => profile.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("SYSDATETIME()");

			builder.HasIndex(profile => profile.UserId).IsUnique();

			builder
				.HasOne(profile => profile.User)
				.WithOne(user => user.FreelancerProfile)
				.HasForeignKey<FreelancerProfile>(profile => profile.UserId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
