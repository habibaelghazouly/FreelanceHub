using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("jobs", table =>
            {
                table.HasCheckConstraint("chk_jobs_budget", "[budget] >= 0");
                table.HasCheckConstraint("chk_jobs_status", "[job_status] IN (40, 41, 42, 43)");
                table.HasCheckConstraint("chk_jobs_deleted_at", "([is_deleted] = 0 AND [deleted_at] IS NULL) OR ([is_deleted] = 1 AND [deleted_at] IS NOT NULL)");
            });

            builder.HasKey(job => job.JobId);

            builder.Property(job => job.JobId).HasColumnName("job_id");
            builder.Property(job => job.ClientUserId).HasColumnName("client_user_id").IsRequired();
            builder.Property(job => job.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
            builder.Property(job => job.Description).HasColumnName("description").IsRequired();
            builder.Property(job => job.Budget).HasColumnName("budget").HasPrecision(18, 2).IsRequired();
            builder.Property(job => job.Deadline).HasColumnName("deadline").IsRequired();
            builder.Property(job => job.JobStatus).HasColumnName("job_status").HasConversion<int>().HasDefaultValue(JobStatus.Open).HasSentinel((JobStatus)0).IsRequired();
            builder.Property(job => job.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
            builder.Property(job => job.DeletedAt).HasColumnName("deleted_at");
            builder.Property(job => job.CreatedAt).HasColumnName("created_at").IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property(job => job.UpdatedAt).HasColumnName("updated_at").IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasQueryFilter(job => !job.IsDeleted);

            builder
                .HasOne(job => job.ClientUser)
                .WithMany(user => user.Jobs)
                .HasForeignKey(job => job.ClientUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(job => job.Contract)
                .WithOne(contract => contract.Job)
                .HasForeignKey<Contract>(contract => contract.JobId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
