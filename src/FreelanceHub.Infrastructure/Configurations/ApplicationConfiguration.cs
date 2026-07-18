using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable("applications");
            builder.HasKey(application => application.ApplicationId);
            builder.Property(application => application.ApplicationId).HasColumnName("application_id");
            builder.Property(application => application.JobId).HasColumnName("job_id").IsRequired();
            builder.Property(application => application.FreelancerProfileId).HasColumnName("freelancer_profile_id").IsRequired();
            builder.Property(application => application.ProposedAmount).HasColumnName("proposed_amount").HasPrecision(18, 2).IsRequired();
            builder.Property(application => application.CoverLetter).HasColumnName("cover_letter").IsRequired();
            builder.Property(application => application.ApplicationStatus).HasColumnName("application_status").IsRequired();
            builder.Property(application => application.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.Property(application => application.UpdatedAt).HasColumnName("updated_at").IsRequired();
            builder.Property(application => application.TimelineDays).HasColumnName("timeline_days").IsRequired();
            // Relationships
            builder.HasOne(application => application.Job)
                .WithMany(job => job.Applications)
                .HasForeignKey(application => application.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(application => application.FreelancerProfile)
                .WithMany(freelancerProfile => freelancerProfile.Applications)
                .HasForeignKey(application => application.FreelancerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
