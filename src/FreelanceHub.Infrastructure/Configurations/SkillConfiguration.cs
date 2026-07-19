using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations.Skills
{
    public class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.ToTable("skills");

            builder.HasKey(skill => skill.SkillId);
            builder.Property(skill => skill.SkillId).HasColumnName("skill_id");

            builder.Property(skill => skill.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(skill => skill.Name).IsUnique();

            builder.HasData(Skills);
        }

        private static readonly Skill[] Skills =
        {
            // Web development
            new() { SkillId = 1, Name = "JavaScript" },
            new() { SkillId = 2, Name = "TypeScript" },
            new() { SkillId = 3, Name = "React" },
            new() { SkillId = 4, Name = "Angular" },
            new() { SkillId = 5, Name = "Vue.js" },
            new() { SkillId = 6, Name = "Node.js" },
            new() { SkillId = 7, Name = "ASP.NET Core" },
            new() { SkillId = 8, Name = "PHP" },
            new() { SkillId = 9, Name = "Laravel" },
            new() { SkillId = 10, Name = "Python" },
            new() { SkillId = 11, Name = "Django" },
            new() { SkillId = 12, Name = "Ruby on Rails" },
            new() { SkillId = 13, Name = "WordPress" },
            new() { SkillId = 14, Name = "HTML/CSS" },

            // Mobile development
            new() { SkillId = 15, Name = "Swift" },
            new() { SkillId = 16, Name = "Kotlin" },
            new() { SkillId = 17, Name = "Flutter" },
            new() { SkillId = 18, Name = "React Native" },

            // Design
            new() { SkillId = 19, Name = "UI/UX Design" },
            new() { SkillId = 20, Name = "Figma" },
            new() { SkillId = 21, Name = "Adobe Photoshop" },
            new() { SkillId = 22, Name = "Adobe Illustrator" },
            new() { SkillId = 23, Name = "Graphic Design" },
            new() { SkillId = 24, Name = "Logo Design" },

            // Writing & content
            new() { SkillId = 25, Name = "Content Writing" },
            new() { SkillId = 26, Name = "Copywriting" },
            new() { SkillId = 27, Name = "Technical Writing" },
            new() { SkillId = 28, Name = "Translation" },
            new() { SkillId = 29, Name = "Editing & Proofreading" },

            // Marketing
            new() { SkillId = 30, Name = "SEO" },
            new() { SkillId = 31, Name = "Social Media Marketing" },
            new() { SkillId = 32, Name = "Email Marketing" },
            new() { SkillId = 33, Name = "Google Ads" },

            // Data & analytics
            new() { SkillId = 34, Name = "Data Analysis" },
            new() { SkillId = 35, Name = "SQL" },
            new() { SkillId = 36, Name = "Machine Learning" },
            new() { SkillId = 37, Name = "Power BI" },

            // DevOps & cloud
            new() { SkillId = 38, Name = "Docker" },
            new() { SkillId = 39, Name = "Kubernetes" },
            new() { SkillId = 40, Name = "AWS" },
            new() { SkillId = 41, Name = "Azure" },
            new() { SkillId = 42, Name = "CI/CD" },

            // Business & admin
            new() { SkillId = 43, Name = "Project Management" },
            new() { SkillId = 44, Name = "Virtual Assistance" },
            new() { SkillId = 45, Name = "Bookkeeping" },

            // Video & audio
            new() { SkillId = 46, Name = "Video Editing" },
            new() { SkillId = 47, Name = "Voice Over" },
            new() { SkillId = 48, Name = "Motion Graphics" }
        };
    }
}
