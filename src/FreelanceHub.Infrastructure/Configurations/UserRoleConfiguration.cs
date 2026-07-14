using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
	public class UserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
	{
		public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
		{
			builder.ToTable("user_roles");
			builder.Property(userRole => userRole.UserId).HasColumnName("user_id");
			builder.Property(userRole => userRole.RoleId).HasColumnName("role_id");
			builder.Property(userRole => userRole.AssignedAt).HasColumnName("assigned_at").HasDefaultValueSql("SYSDATETIME()");
		}
	}
}
