using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
	public class JobTagConfiguration : IEntityTypeConfiguration<JobTag>
	{
		public void Configure(EntityTypeBuilder<JobTag> builder)
		{
			builder.ToTable("job_tags");

			builder.HasKey(jobTag => new { jobTag.JobId, jobTag.TagId });

			builder.Property(jobTag => jobTag.JobId).HasColumnName("job_id");
			builder.Property(jobTag => jobTag.TagId).HasColumnName("tag_id");

			builder.HasQueryFilter(jobTag => !jobTag.Job.IsDeleted);

			builder.HasOne(jobTag => jobTag.Job)
				.WithMany(job => job.JobTags)
				.HasForeignKey(jobTag => jobTag.JobId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(jobTag => jobTag.Tag)
				.WithMany(tag => tag.JobTags)
				.HasForeignKey(jobTag => jobTag.TagId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
