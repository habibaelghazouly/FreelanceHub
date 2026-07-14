using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
	public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<int>>
	{
		public void Configure(EntityTypeBuilder<IdentityRole<int>> builder)
		{
			builder.ToTable("roles");
			builder.Property(role => role.Id).HasColumnName("role_id");
			builder.Property(role => role.Name).HasColumnName("name").HasMaxLength(30).IsRequired();
			builder.Property(role => role.NormalizedName).HasColumnName("normalized_name").HasMaxLength(30);

			builder.HasData(
				new { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
				new { Id = 2, Name = "Client", NormalizedName = "CLIENT" },
				new { Id = 3, Name = "Freelancer", NormalizedName = "FREELANCER" });
		}
	}
}
