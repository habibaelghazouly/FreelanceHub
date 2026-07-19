using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable("applications", table =>
                table.HasCheckConstraint("chk_applications_status", "[application_status] IN (50, 51, 52, 53, 54)"));
            builder.HasKey(application => application.ApplicationId);
            builder.HasAlternateKey(application => new { application.ApplicationId, application.JobId });
            builder.Property(application => application.ApplicationId).HasColumnName("application_id");
            builder.Property(application => application.JobId).HasColumnName("job_id").IsRequired();
            builder.Property(application => application.FreelancerUserId).HasColumnName("freelancer_user_id").IsRequired();          
            builder.Property(application => application.ProposedAmount).HasColumnName("proposed_amount").HasPrecision(18, 2).IsRequired();
            builder.Property(application => application.CoverLetter).HasColumnName("cover_letter").IsRequired();
            builder.Property(application => application.ApplicationStatus).HasColumnName("application_status").HasConversion<int>().HasDefaultValue(ApplicationStatus.Submitted).HasSentinel((ApplicationStatus)0).IsRequired();
            builder.Property(application => application.CreatedAt).HasColumnName("created_at").IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property(application => application.UpdatedAt).HasColumnName("updated_at").IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property(application => application.TimelineDays).HasColumnName("timeline_days").IsRequired();

            builder.HasQueryFilter(application => !application.Job.IsDeleted);
            // Relationships
            builder.HasOne(application => application.Job)
                .WithMany(job => job.Applications)
                .HasForeignKey(application => application.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(application => application.FreelancerUser)
                .WithMany(freelancerUser => freelancerUser.Applications)
                .HasForeignKey(application => application.FreelancerUserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
