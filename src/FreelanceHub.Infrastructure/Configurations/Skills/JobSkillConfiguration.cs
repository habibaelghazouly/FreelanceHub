using FreelanceHub.Domain.Models.Skills;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations.Skills
{
	public class JobSkillConfiguration : IEntityTypeConfiguration<JobSkill>
	{
		public void Configure(EntityTypeBuilder<JobSkill> builder)
		{
			builder.ToTable("job_skills");

			builder.HasKey(js => new { js.JobId, js.SkillId });

			builder.Property(js => js.JobId).HasColumnName("job_id");
			builder.Property(js => js.SkillId).HasColumnName("skill_id");

			// Requires the Job entity (not included in this delivery).
			// builder.HasOne(js => js.Job)
			// 	.WithMany(job => job.JobSkills)
			// 	.HasForeignKey(js => js.JobId)
			// 	.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(js => js.Skill)
				.WithMany(skill => skill.JobSkills)
				.HasForeignKey(js => js.SkillId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
