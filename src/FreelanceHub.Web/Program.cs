using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Application.Services.Implementations;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using FreelanceHub.Infrastructure.Repositories.Implementations;
using FreelanceHub.Infrastructure.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			builder.Services
				.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
				{
					options.User.RequireUniqueEmail = true;
					options.Password.RequiredLength = 6;
					options.Password.RequireDigit = false;
					options.Password.RequireLowercase = true;
					options.Password.RequireUppercase = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
					options.Lockout.MaxFailedAccessAttempts = 5;
				})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/Account/Login";
				options.AccessDeniedPath = "/Account/AccessDenied";
				options.SlidingExpiration = true;
				options.ExpireTimeSpan = TimeSpan.FromDays(7);
			});

			builder.Services.AddControllersWithViews();

			var webRootPath = builder.Environment.WebRootPath ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
			builder.Services.AddSingleton(new FileStorageOptions
			{
				RootPath = webRootPath,
				PublicBasePath = "uploads"
			});

			builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();
			builder.Services.AddScoped<IFileUploadService, FileUploadService>();
			builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
			builder.Services.AddScoped<IClientProfileRepository, ClientProfileRepository>();
			builder.Services.AddScoped<IFreelancerProfileRepository, FreelancerProfileRepository>();
			builder.Services.AddScoped<IFileStorageRepository, FileStorageRepository>();

            builder.Services.AddScoped<IApplicationManagementService, ApplicationManagementService>();
            builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();

            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
