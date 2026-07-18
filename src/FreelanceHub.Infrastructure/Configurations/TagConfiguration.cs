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
            builder.HasKey(tag => tag.Id);
            builder.Property(tag => tag.Id).HasColumnName("tag_id");
            builder.Property(tag => tag.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

            builder.HasData(
                new  { Id = 1, Name = "C#" },
                new  { Id = 2, Name = ".NET" },
                new  { Id = 3, Name = "React" },
                new  { Id = 4, Name = "SQL" },
                new  { Id = 5, Name = "Figma" },
                new  { Id = 6, Name = "SEO" },
                new  { Id = 7, Name = "API" },
                new  { Id = 8, Name = "Content Writing" });
        }
    }
}