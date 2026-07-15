using FreelanceHub.Domain.Models.Skills;
using FreelanceHub.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations.Skills
{
	public class SkillConfiguration : IEntityTypeConfiguration<Skill>
	{
		public void Configure(EntityTypeBuilder<Skill> builder)
		{
			builder.ToTable("skills");

			builder.HasKey(skill => skill.Id);
			builder.Property(skill => skill.Id).HasColumnName("skill_id");

			builder.Property(skill => skill.Name)
				.HasColumnName("name")
				.HasMaxLength(100)
				.IsRequired();

			builder.HasIndex(skill => skill.Name).IsUnique();

			builder.HasData(SkillSeedData.Skills);
		}
	}
}
