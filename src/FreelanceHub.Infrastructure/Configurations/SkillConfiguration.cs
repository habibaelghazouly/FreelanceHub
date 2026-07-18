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

            builder.HasKey(skill => skill.Id);
            builder.Property(skill => skill.Id).HasColumnName("skill_id");

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
            new() { Id = 1, Name = "JavaScript" },
            new() { Id = 2, Name = "TypeScript" },
            new() { Id = 3, Name = "React" },
            new() { Id = 4, Name = "Angular" },
            new() { Id = 5, Name = "Vue.js" },
            new() { Id = 6, Name = "Node.js" },
            new() { Id = 7, Name = "ASP.NET Core" },
            new() { Id = 8, Name = "PHP" },
            new() { Id = 9, Name = "Laravel" },
            new() { Id = 10, Name = "Python" },
            new() { Id = 11, Name = "Django" },
            new() { Id = 12, Name = "Ruby on Rails" },
            new() { Id = 13, Name = "WordPress" },
            new() { Id = 14, Name = "HTML/CSS" },

            // Mobile development
            new() { Id = 15, Name = "Swift" },
            new() { Id = 16, Name = "Kotlin" },
            new() { Id = 17, Name = "Flutter" },
            new() { Id = 18, Name = "React Native" },

            // Design
            new() { Id = 19, Name = "UI/UX Design" },
            new() { Id = 20, Name = "Figma" },
            new() { Id = 21, Name = "Adobe Photoshop" },
            new() { Id = 22, Name = "Adobe Illustrator" },
            new() { Id = 23, Name = "Graphic Design" },
            new() { Id = 24, Name = "Logo Design" },

            // Writing & content
            new() { Id = 25, Name = "Content Writing" },
            new() { Id = 26, Name = "Copywriting" },
            new() { Id = 27, Name = "Technical Writing" },
            new() { Id = 28, Name = "Translation" },
            new() { Id = 29, Name = "Editing & Proofreading" },

            // Marketing
            new() { Id = 30, Name = "SEO" },
            new() { Id = 31, Name = "Social Media Marketing" },
            new() { Id = 32, Name = "Email Marketing" },
            new() { Id = 33, Name = "Google Ads" },

            // Data & analytics
            new() { Id = 34, Name = "Data Analysis" },
            new() { Id = 35, Name = "SQL" },
            new() { Id = 36, Name = "Machine Learning" },
            new() { Id = 37, Name = "Power BI" },

            // DevOps & cloud
            new() { Id = 38, Name = "Docker" },
            new() { Id = 39, Name = "Kubernetes" },
            new() { Id = 40, Name = "AWS" },
            new() { Id = 41, Name = "Azure" },
            new() { Id = 42, Name = "CI/CD" },

            // Business & admin
            new() { Id = 43, Name = "Project Management" },
            new() { Id = 44, Name = "Virtual Assistance" },
            new() { Id = 45, Name = "Bookkeeping" },

            // Video & audio
            new() { Id = 46, Name = "Video Editing" },
            new() { Id = 47, Name = "Voice Over" },
            new() { Id = 48, Name = "Motion Graphics" }
        };
    }
}