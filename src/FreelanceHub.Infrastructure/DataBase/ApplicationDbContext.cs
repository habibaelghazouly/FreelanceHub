using FreelanceHub.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.DataBase
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int, IdentityUserClaim<int>, ApplicationUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Attachment> Attachments { get; set; }

		public DbSet<ClientProfile> ClientProfiles { get; set; }

		public DbSet<FreelancerProfile> FreelancerProfiles { get; set; }

		public DbSet<Job> Jobs { get; set; }

		public DbSet<Contract> Contracts { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

			builder.Entity<IdentityUserClaim<int>>().ToTable("user_claims");
			builder.Entity<IdentityUserLogin<int>>().ToTable("user_logins");
			builder.Entity<IdentityRoleClaim<int>>().ToTable("role_claims");
			builder.Entity<IdentityUserToken<int>>().ToTable("user_tokens");
		}
	}
}
