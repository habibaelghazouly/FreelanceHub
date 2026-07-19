using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FreelanceHub.Domain.Models;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("tags");
            builder.HasKey(tag => tag.TagId);
            builder.Property(tag => tag.TagId).HasColumnName("tag_id");
            builder.Property(tag => tag.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

            builder.HasIndex(tag => tag.Name).IsUnique();

            builder.HasData(
                new { TagId = 1, Name = "C#" },
                new { TagId = 2, Name = ".NET" },
                new { TagId = 3, Name = "React" },
                new { TagId = 4, Name = "SQL" },
                new { TagId = 5, Name = "Figma" },
                new { TagId = 6, Name = "SEO" },
                new { TagId = 7, Name = "API" },
                new { TagId = 8, Name = "Content Writing" });
        }
    }
}
