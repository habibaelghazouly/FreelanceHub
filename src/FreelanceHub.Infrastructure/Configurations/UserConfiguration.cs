using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
	public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			builder.ToTable("users");
			builder.Property(user => user.Id).HasColumnName("user_id");
			builder.Property(user => user.UserName).HasColumnName("username").HasMaxLength(50).IsRequired();
			builder.Property(user => user.NormalizedUserName).HasColumnName("normalized_username").HasMaxLength(50);
			builder.Property(user => user.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
			builder.Property(user => user.NormalizedEmail).HasColumnName("normalized_email").HasMaxLength(255);
			builder.Property(user => user.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
			builder.Property(user => user.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
			builder.Property(user => user.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
			builder.Property(user => user.UserStatus).HasColumnName("user_status");
			builder.Property(user => user.EmailConfirmed).HasColumnName("is_email_verified");
			builder.Property(user => user.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSDATETIME()");
			builder.Property(user => user.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("SYSDATETIME()");
			builder.HasIndex(user => user.NormalizedEmail).HasDatabaseName("EmailIndex").IsUnique().HasFilter("[normalized_email] IS NOT NULL");
		}
	}
}
