using System.Security.Claims;
using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FreelanceHub.Infrastructure.Repositories.Abstractions;

namespace FreelanceHub.Web.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationManagementService _applicationManagementService;
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationsController(IApplicationManagementService applicationManagementService, IApplicationRepository applicationRepository  )
        {
            _applicationManagementService = applicationManagementService;
            _applicationRepository = applicationRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> Submit(int jobId)
        {

            if (jobId <= 0)
            {
                return BadRequest("A valid Job ID is required to apply.");
            }
            if (!TryGetCurrentUserId(out var freelancerUserId))
            {
                return Forbid();
            }

            var job = await _applicationManagementService.GetOpenJobByIdAsync(jobId, HttpContext.RequestAborted);
            if (job is null)
            {
                return NotFound("The requested job is not available for application.");
            }
            var alreadyApplied = await _applicationRepository.HasFreelancerAppliedAsync(jobId, freelancerUserId, HttpContext.RequestAborted);

            if (alreadyApplied)
            {
                
                ViewBag.JobTitle = job.Title;
                return View("AlreadySubmitted");
            }

            return View(new SubmitApplicationViewModel
            {
                JobId = job.JobId,
                JobTitle = job.Title
            });
        }

        [HttpPost]
        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> Submit(SubmitApplicationViewModel model)
        {
            if (!TryGetCurrentUserId(out var freelancerUserId))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var openedStreams = new List<Stream>();
            try
            {
                var portfolioFiles = new List<UploadedFileRequest>();
                foreach (var portfolioFile in model.PortfolioFiles.Where(file => file.Length > 0))
                {
                    var stream = portfolioFile.OpenReadStream();
                    openedStreams.Add(stream);
                    portfolioFiles.Add(new UploadedFileRequest(stream, portfolioFile.FileName, portfolioFile.ContentType, portfolioFile.Length));
                }

                var result = await _applicationManagementService.SubmitApplicationAsync(new SubmitApplicationRequest
                {
                    JobId = model.JobId,
                    FreelancerUserId = freelancerUserId,
                    ProposedAmount = model.ProposedAmount!.Value,
                    CoverLetter = model.CoverLetter,
                    TimelineDays = model.TimelineDays!.Value,
                    PortfolioFiles = portfolioFiles
                }, HttpContext.RequestAborted);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }

                    return View(model);
                }

                TempData["SuccessMessage"] = "Application submitted successfully.";
                return RedirectToAction(nameof(MyApplications));
            }
            finally
            {
                foreach (var stream in openedStreams)
                {
                    stream.Dispose();
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> MyApplications()
        {
            if (!TryGetCurrentUserId(out var freelancerUserId))
            {
                return Forbid();
            }

            var dashboard = await _applicationManagementService.GetFreelancerDashboardAsync(freelancerUserId, HttpContext.RequestAborted);

            return View(new FreelancerApplicationDashboardViewModel
            {
                Applications = dashboard.Applications.Select(item => new FreelancerApplicationItemViewModel
                {
                    ApplicationId = item.ApplicationId,
                    JobId = item.JobId,
                    JobTitle = item.JobTitle,
                    ProposedAmount = item.ProposedAmount,
                    TimelineDays = item.TimelineDays,
                    ApplicationStatus = item.ApplicationStatus,
                    PortfolioItemCount = item.PortfolioItemCount,
                    SubmittedAt = item.CreatedAt
                }).ToArray()
            });
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> SubmittedApplications(int jobId )
        {

            // Require a valid jobId
            if (jobId <= 0)
            {
                return BadRequest("A valid Job ID is required to view submitted applications.");
            }

            if (!TryGetCurrentUserId(out var clientUserId))
            {
                return Forbid();
            }

            var dashboard = await _applicationManagementService.GetClientDashboardAsync(clientUserId, jobId, HttpContext.RequestAborted);
            return View(new ClientApplicationDashboardViewModel
            {
                Applications = dashboard.Applications.Select(item => new ClientApplicationItemViewModel
                {
                    ApplicationId = item.ApplicationId,
                    JobId = item.JobId,
                    JobTitle = item.JobTitle,
                    FreelancerUserId = item.FreelancerUserId,
                    FreelancerDisplayName = item.FreelancerDisplayName,
                    ProposedAmount = item.ProposedAmount,
                    TimelineDays = item.TimelineDays,
                    ApplicationStatus = item.ApplicationStatus,
                    SubmittedAt = item.SubmittedAt
                }).ToArray()
            });
        }


        private bool TryGetCurrentUserId(out int userId)
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claimValue, out userId);
        }
    }
}
