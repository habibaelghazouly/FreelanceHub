using FreelanceHub.Domain.Enums;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using FreelanceHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private const string FreelancerRole = "Freelancer";
        private const string ClientRole = "Client";
        private readonly IAdminRepository _adminRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IAdminRepository adminRepository, IUnitOfWork unitOfWork)
        {
            _adminRepository = adminRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var freelancerUsers = await _adminRepository.ListUsersInRoleAsync(FreelancerRole);
            var clientUsers = await _adminRepository.ListUsersInRoleAsync(ClientRole);
            var jobEntities = await _adminRepository.ListJobsAsync();
            var contractEntities = await _adminRepository.ListContractsAsync();

            var freelancers = freelancerUsers.Select(user => new AdminUserViewModel { UserId = user.Id, Name = (user.FirstName + " " + user.LastName).Trim(), Email = user.Email ?? string.Empty, Status = user.UserStatus.ToString() }).ToList();
            var clients = clientUsers.Select(user => new AdminUserViewModel { UserId = user.Id, Name = (user.FirstName + " " + user.LastName).Trim(), Email = user.Email ?? string.Empty, Status = user.UserStatus.ToString() }).ToList();
            var jobs = jobEntities.Select(job => new AdminJobViewModel
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
                }).ToList();

            var contracts = contractEntities.Select(contract => new AdminContractViewModel
                {
                    ContractId = contract.ContractId,
                    JobTitle = contract.Job.Title,
                    ClientName = (contract.Job.ClientUser.FirstName + " " + contract.Job.ClientUser.LastName).Trim(),
                    FreelancerName = (contract.AcceptedApplication.FreelancerUser.FirstName + " " + contract.AcceptedApplication.FreelancerUser.LastName).Trim(),
                    Amount = contract.AgreedAmount,
                    Status = contract.ContractStatus.ToString()
                }).ToList();

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

        [HttpGet]
        public async Task<IActionResult> Contracts()
        {
            var contracts = await _adminRepository.ListContractsAsync(HttpContext.RequestAborted);

            return View(contracts.Select(contract => new AdminContractViewModel
            {
                ContractId = contract.ContractId,
                JobTitle = contract.Job.Title,
                ClientName = (contract.Job.ClientUser.FirstName + " " + contract.Job.ClientUser.LastName).Trim(),
                FreelancerName = (contract.AcceptedApplication.FreelancerUser.FirstName + " " + contract.AcceptedApplication.FreelancerUser.LastName).Trim(),
                Amount = contract.AgreedAmount,
                Status = contract.ContractStatus.ToString()
            }).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeJob(int id)
        {
            if (id <= 0) return BadRequest();
            var job = await _adminRepository.GetJobForRevocationAsync(id);

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
            await _unitOfWork.SaveChangesAsync();

            TempData["AdminSuccess"] = "The job has been revoked and is no longer available to freelancers.";
            return RedirectToAction(nameof(Index));
        }
    }
}
