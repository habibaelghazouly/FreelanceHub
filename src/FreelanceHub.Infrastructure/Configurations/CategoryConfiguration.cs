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
            builder.HasKey(category => category.CategoryId);
            builder.Property(category => category.CategoryId).HasColumnName("category_id");
            builder.Property(category => category.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

            builder.HasIndex(category => category.Name).IsUnique();

            builder.HasData(
                new Category { CategoryId = 1, Name = "Web Development" },
                new Category { CategoryId = 2, Name = "Mobile Development" },
                new Category { CategoryId = 3, Name = "UI / UX Design" },
                new Category { CategoryId = 4, Name = "Writing" },
                new Category { CategoryId = 5, Name = "Marketing" });
        }
    }
}
