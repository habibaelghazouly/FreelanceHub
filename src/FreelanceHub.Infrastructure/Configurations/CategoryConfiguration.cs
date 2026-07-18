using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories");
            builder.HasKey(category => category.Id);
            builder.Property(category => category.Id).HasColumnName("category_id");
            builder.Property(category => category.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

            builder.HasData(
                new Category { Id = 1, Name = "Web Development" },
                new Category { Id = 2, Name = "Mobile Development" },
                new Category { Id = 3, Name = "UI / UX Design" },
                new Category { Id = 4, Name = "Writing" },
                new Category { Id = 5, Name = "Marketing" });
        }
    }
}