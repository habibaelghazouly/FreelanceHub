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

		public DbSet<Skill> Skills => Set<Skill>();

		public DbSet<Attachment> Attachments => Set<Attachment>();

		public DbSet<FreelancerSkill> FreelancerSkills => Set<FreelancerSkill>();

		public DbSet<JobSkill> JobSkills => Set<JobSkill>();

		public DbSet<FreelancerProfileAttachment> FreelancerProfileAttachments => Set<FreelancerProfileAttachment>();

		public DbSet<ClientProfileAttachment> ClientProfileAttachments => Set<ClientProfileAttachment>();

		public DbSet<JobAttachment> JobAttachments => Set<JobAttachment>();

		public DbSet<ApplicationAttachment> ApplicationAttachments => Set<ApplicationAttachment>();

		public DbSet<ContractAttachment> ContractAttachments => Set<ContractAttachment>();

		public DbSet<FreelancerProfile> FreelancerProfiles => Set<FreelancerProfile>();
		public DbSet<ClientProfile> ClientProfiles => Set<ClientProfile>();
		public DbSet<Job> Jobs => Set<Job>();
		public DbSet<Contract> Contracts => Set<Contract>();

		public DbSet<Application> Applications => Set<Application>();

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
