using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
	public class JobCategoryConfiguration : IEntityTypeConfiguration<JobCategory>
	{
		public void Configure(EntityTypeBuilder<JobCategory> builder)
		{
			builder.ToTable("job_categories");

			builder.HasKey(jobCategory => new { jobCategory.JobId, jobCategory.CategoryId });

			builder.Property(jobCategory => jobCategory.JobId).HasColumnName("job_id");
			builder.Property(jobCategory => jobCategory.CategoryId).HasColumnName("category_id");

			builder.HasQueryFilter(jobCategory => !jobCategory.Job.IsDeleted);

			builder.HasOne(jobCategory => jobCategory.Job)
				.WithMany(job => job.JobCategories)
				.HasForeignKey(jobCategory => jobCategory.JobId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(jobCategory => jobCategory.Category)
				.WithMany(category => category.JobCategories)
				.HasForeignKey(jobCategory => jobCategory.CategoryId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
