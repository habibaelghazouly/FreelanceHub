using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations.Skills
{
	public class FreelancerSkillConfiguration : IEntityTypeConfiguration<FreelancerSkill>
	{
		public void Configure(EntityTypeBuilder<FreelancerSkill> builder)
		{
			builder.ToTable("freelancer_skills");

			builder.HasKey(fs => new { fs.FreelancerProfileId, fs.SkillId });

			builder.Property(fs => fs.FreelancerProfileId).HasColumnName("freelancer_profile_id");
			builder.Property(fs => fs.SkillId).HasColumnName("skill_id");

			// Requires the FreelancerProfile entity (not included in this delivery).
			// builder.HasOne(fs => fs.FreelancerProfile)
			// 	.WithMany(profile => profile.FreelancerSkills)
			// 	.HasForeignKey(fs => fs.FreelancerProfileId)
			// 	.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(fs => fs.Skill)
				.WithMany(skill => skill.FreelancerSkills)
				.HasForeignKey(fs => fs.SkillId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
