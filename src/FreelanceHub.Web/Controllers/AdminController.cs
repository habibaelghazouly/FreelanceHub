using FreelanceHub.Domain.Enums;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private const string FreelancerRole = "Freelancer";
        private const string ClientRole = "Client";
        private readonly ApplicationDbContext _dbContext;

        public AdminController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var roleAssignments = _dbContext.UserRoles.Join(
                _dbContext.Roles,
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => new { userRole.UserId, role.Name });

            var freelancers = await roleAssignments
                .Where(item => item.Name == FreelancerRole)
                .Join(_dbContext.Users, item => item.UserId, user => user.Id, (item, user) => new AdminUserViewModel
                {
                    UserId = user.Id,
                    Name = (user.FirstName + " " + user.LastName).Trim(),
                    Email = user.Email ?? string.Empty,
                    Status = user.UserStatus.ToString()
                })
                .OrderBy(user => user.Name)
                .ToListAsync();

            var clients = await roleAssignments
                .Where(item => item.Name == ClientRole)
                .Join(_dbContext.Users, item => item.UserId, user => user.Id, (item, user) => new AdminUserViewModel
                {
                    UserId = user.Id,
                    Name = (user.FirstName + " " + user.LastName).Trim(),
                    Email = user.Email ?? string.Empty,
                    Status = user.UserStatus.ToString()
                })
                .OrderBy(user => user.Name)
                .ToListAsync();

            var jobs = await _dbContext.Jobs
                .AsNoTracking()
                .Include(job => job.ClientUser)
                .Include(job => job.Applications)
                .Include(job => job.Contract)
                .OrderByDescending(job => job.CreatedAt)
                .Select(job => new AdminJobViewModel
                {
                    JobId = job.JobId,
                    Title = job.Title,
                    ClientName = (job.ClientUser.FirstName + " " + job.ClientUser.LastName).Trim(),
                    Budget = job.Budget,
                    Status = job.IsDeleted ? "Revoked" : job.JobStatus.ToString(),
                    ApplicationCount = job.Applications.Count,
                    CanRevoke = !job.IsDeleted
                        && job.Contract == null
                        && !job.Applications.Any(application => application.ApplicationStatus == ApplicationStatus.Accepted)
                })
                .ToListAsync();

            var contracts = await _dbContext.Contracts
                .AsNoTracking()
                .Include(contract => contract.Job)
                    .ThenInclude(job => job.ClientUser)
                .Include(contract => contract.AcceptedApplication)
                    .ThenInclude(application => application.FreelancerUser)
                .OrderByDescending(contract => contract.StartDate)
                .Select(contract => new AdminContractViewModel
                {
                    ContractId = contract.ContractId,
                    JobTitle = contract.Job.Title,
                    ClientName = (contract.Job.ClientUser.FirstName + " " + contract.Job.ClientUser.LastName).Trim(),
                    FreelancerName = (contract.AcceptedApplication.FreelancerUser.FirstName + " " + contract.AcceptedApplication.FreelancerUser.LastName).Trim(),
                    Amount = contract.AgreedAmount,
                    Status = contract.ContractStatus.ToString()
                })
                .ToListAsync();

            return View(new AdminDashboardViewModel
            {
                FreelancerCount = freelancers.Count,
                ClientCount = clients.Count,
                JobCount = jobs.Count,
                ContractCount = contracts.Count,
                Freelancers = freelancers,
                Clients = clients,
                Jobs = jobs,
                Contracts = contracts
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeJob(int id)
        {
            var job = await _dbContext.Jobs
                .Include(item => item.Applications)
                .Include(item => item.Contract)
                .SingleOrDefaultAsync(item => item.JobId == id);

            if (job is null)
            {
                return NotFound();
            }

            var hasAcceptedApplication = job.Applications.Any(application => application.ApplicationStatus == ApplicationStatus.Accepted);
            if (job.IsDeleted || job.Contract is not null || hasAcceptedApplication)
            {
                TempData["AdminError"] = "A job with an accepted application cannot be revoked.";
                return RedirectToAction(nameof(Index));
            }

            job.IsDeleted = true;
            job.DeletedAt = DateTime.UtcNow;
            job.JobStatus = JobStatus.Cancelled;
            job.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            TempData["AdminSuccess"] = "The job has been revoked and is no longer available to freelancers.";
            return RedirectToAction(nameof(Index));
        }
    }
}
